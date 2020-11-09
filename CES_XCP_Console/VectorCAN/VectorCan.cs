using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vxlapi_NET;

namespace CES_XCP_Console.VectorCAN
{

    class VectorCan
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(int handle, int timeOut);

        private XLDriver CANDriver;
        private const String appName = "CES_XCP_Console";
        private String info;

        private XLDefine.XL_Status status;

        // Driver configuration
        private XLClass.xl_driver_config driverConfig;

        // Variables required by XLDriver
        private XLDefine.XL_HardwareType hwType;
        private uint hwIndex = 0;
        private uint hwChannel = 0;
        private int portHandle = -1;
        private int eventHandle = -1;
        private UInt64 accessMask = 0;
        private UInt64 permissionMask = 0;
        private UInt64 txMask = 0;
        private UInt64 rxMask = 0;
        private int txCi = 0;
        private int rxCi = 0;
        private EventWaitHandle xlEvWaitHandle;

        private uint canFdModeNoIso = 0;      // Global CAN FD ISO (default) / no ISO mode flag

        // RX thread
        private Thread rxThread;
        private bool blockRxThread = false;

        private String rawRxString;

        public VectorCan()
        {
            CANDriver = new XLDriver();
            driverConfig = new XLClass.xl_driver_config();
            hwType = XLDefine.XL_HardwareType.XL_HWTYPE_NONE;
            xlEvWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, null);
            info = "vxlapi_NET        : " + typeof(XLDriver).Assembly.GetName().Version+"\n";
        }

        public String RawRxString
        {
            get
            {
                return rawRxString;
            }
        }

        public bool OpenDriver()
        {
            status = CANDriver.XL_OpenDriver();
            info += "Open Driver       : ";
            if (status == XLDefine.XL_Status.XL_SUCCESS)
            {
                info += "Succesfull\n";
                status = CANDriver.XL_GetDriverConfig(ref driverConfig);
                info += "DLL Version       : " + CANDriver.VersionToString(driverConfig.dllVersion) + "\n"+
                    "Channels found    : " + driverConfig.channelCount + "\n";

                for (int i = 0; i < driverConfig.channelCount; i++)
                {
                    info += "       " + driverConfig.channel[i].name + "\n";

                    if ((driverConfig.channel[i].channelCapabilities & XLDefine.XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT) == XLDefine.XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT)
                        info += "       - CAN FD Support  : yes\n";
                    else
                        info += "        - CAN FD Support  : no\n";

                    info += "                    - Channel Mask    : " + driverConfig.channel[i].channelMask + "\n";
                    info += "                    - Transceiver Name: " + driverConfig.channel[i].transceiverName+"\n";
                    info += "                    - Serial Number: " + driverConfig.channel[i].serialNumber + "\n";

                }

                // If the application name cannot be found in VCANCONF...
                if ((CANDriver.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                    (CANDriver.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS))
                {
                    //...create the item with two CAN channels
                    CANDriver.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                    CANDriver.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                    CANDriver.XL_PopupHwConfig();                
                }

                accessMask = txMask | rxMask;
                permissionMask = accessMask;

                return true;
            }
            else
            {
                info += "NOT Succesfull\n";
                return false;
            }

        }

        public void OpenPort(bool iscanfd, uint arbitSpeed, uint dataSpeed)
        {
            if (iscanfd)
            {
                // --------------------
                // Set CAN FD config
                // --------------------
                status = CANDriver.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 16000, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION_V4, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                info += "\n\nOpen Port             : " + status;
                
                XLClass.XLcanFdConf canFdConf = new XLClass.XLcanFdConf();

                // arbitration bitrate
                canFdConf.arbitrationBitRate = arbitSpeed;
                canFdConf.tseg1Abr = 8;
                canFdConf.tseg2Abr = 7;
                canFdConf.sjwAbr = 1;

                // data bitrate
                canFdConf.dataBitRate = dataSpeed;
                canFdConf.tseg1Dbr = 4;
                canFdConf.tseg2Dbr = 3;
                canFdConf.sjwDbr = 1;

                if (canFdModeNoIso > 0)
                {
                    canFdConf.options = (byte)XLDefine.XL_CANFD_ConfigOptions.XL_CANFD_CONFOPT_NO_ISO;
                }
                else
                {
                    canFdConf.options = 0;
                }

                status = CANDriver.XL_CanFdSetConfiguration(portHandle, accessMask, canFdConf);
                info += "\n\nSet CAN FD Config     : " + status;


                // Get RX event handle
                status = CANDriver.XL_SetNotification(portHandle, ref eventHandle, 1);
                info += "Set Notification      : " + status;


                // Activate channel - with reset clock
                status = CANDriver.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_RESET_CLOCK);
                info += "Activate Channel      : " + status;


                // Get XL Driver configuration to get the actual setup parameter
                status = CANDriver.XL_GetDriverConfig(ref driverConfig);

                if (canFdModeNoIso > 0)
                {
                    info += "CAN FD mode           : NO ISO";
                }
                else
                {
                    info += "CAN FD mode           : ISO";
                }
                info += "TX Arb. BitRate       : " + driverConfig.channel[txCi].busParams.dataCanFd.arbitrationBitRate
                                + "Bd, Data Bitrate: " + driverConfig.channel[txCi].busParams.dataCanFd.dataBitRate + "Bd";
            }
            else
            {
                // --------------------
                // Set CAN config
                // --------------------
                status = CANDriver.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                info += "\n\nOpen Port             : " + status;

                //check port
                status = CANDriver.XL_CanRequestChipState(portHandle, accessMask);
                info += "\nCan Request Chip State: " + status;

                // Activate channel
                status = CANDriver.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
                info += "\nActivate Channel      : " + status;

                // Initialize EventWaitHandle object with RX event handle provided by DLL
                int tempInt = -1;
                status = CANDriver.XL_SetNotification(portHandle, ref tempInt, 1);
                xlEvWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(tempInt), true);

                info += "\nSet Notification      : " + status;               

                // Reset time stamp clock
                status = CANDriver.XL_ResetClock(portHandle);
                info += "\nReset Clock           : " + status;


            }

        }

        public void StartRxThread()
        {
            rxThread = new Thread(new ThreadStart(RXThread));
            rxThread.Start();
        }

        public String Info
        {
            get
            {
                return info;
            }
        }

        private bool GetAppChannelAndTestIsOk(uint appChIdx, ref UInt64 chMask, ref int chIdx)
        {
            XLDefine.XL_Status status = CANDriver.XL_GetApplConfig(appName, appChIdx, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                info += "XL_GetApplConfig      : " + status+ "\n";
            }

            chMask = CANDriver.XL_GetChannelMask(hwType, (int)hwIndex, (int)hwChannel);
            chIdx = CANDriver.XL_GetChannelIndex(hwType, (int)hwIndex, (int)hwChannel);
            if (chIdx < 0 || chIdx >= driverConfig.channelCount)
            {
                return false;
            }

            if ((driverConfig.channel[chIdx].channelBusCapabilities & XLDefine.XL_BusCapabilities.XL_BUS_ACTIVE_CAP_CAN) == 0)
            {
            
                return false;
            }

            if (canFdModeNoIso > 0)
            {
                if ((driverConfig.channel[chIdx].channelCapabilities & XLDefine.XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_BOSCH_SUPPORT) == 0)
                {
                    info += driverConfig.channel[chIdx].name.TrimEnd(' ', '\0') + " " + driverConfig.channel[chIdx].transceiverName.TrimEnd(' ', '\0') + " does not support CAN FD NO-ISO";
                    return false;
                }
            }
            else
            {
                if ((driverConfig.channel[chIdx].channelCapabilities & XLDefine.XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT) == 0)
                {
                    info += driverConfig.channel[chIdx].name.TrimEnd(' ', '\0') + " " + driverConfig.channel[chIdx].transceiverName.TrimEnd(' ', '\0') + " does not support CAN FD ISO";
                    return false;
                }
            }       

            return true;
        }

        public uint CANFDTransmit(XLClass.xl_canfd_event_collection xlEventCollection)
        {
            XLDefine.XL_Status txStatus;

            /*
            xlEventCollection = new XLClass.xl_canfd_event_collection(1);           
            xlEventCollection.xlCANFDEvent[0].tag = XLDefine.XL_CANFD_TX_EventTags.XL_CAN_EV_TAG_TX_MSG;
            xlEventCollection.xlCANFDEvent[0].tagData.canId = 0x80000000 | 0x14420FFF;
            xlEventCollection.xlCANFDEvent[0].tagData.dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
            xlEventCollection.xlCANFDEvent[0].tagData.msgFlags = XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_BRS | XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_EDL;
            xlEventCollection.xlCANFDEvent[0].tagData.data[0] = 0x50;
            xlEventCollection.xlCANFDEvent[0].tagData.data[1] = 0xFF;
            xlEventCollection.xlCANFDEvent[0].tagData.data[2] = 0xFF;
            xlEventCollection.xlCANFDEvent[0].tagData.data[3] = 0xFF;
            xlEventCollection.xlCANFDEvent[0].tagData.data[4] = 0xFF;
            xlEventCollection.xlCANFDEvent[0].tagData.data[5] = 0xFF;
            xlEventCollection.xlCANFDEvent[0].tagData.data[6] = 0xFF;
            xlEventCollection.xlCANFDEvent[0].tagData.data[7] = 0xFF;
            */

           
            uint messageCounterSent = 0;
            txStatus = CANDriver.XL_CanTransmitEx(portHandle, txMask, ref messageCounterSent, xlEventCollection);
            return messageCounterSent;
        }

        public void RXThread()
        {            
            XLClass.XLcanRxEvent receivedEvent = new XLClass.XLcanRxEvent();
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;
            XLDefine.WaitResults waitResult = new XLDefine.WaitResults();

            while (true)
            {
                waitResult = (XLDefine.WaitResults)WaitForSingleObject(eventHandle, 1000);

                // If event occurred...
                if (waitResult != XLDefine.WaitResults.WAIT_TIMEOUT)
                {
                    // ...init xlStatus first
                    xlStatus = XLDefine.XL_Status.XL_SUCCESS;

                    // afterwards: while hw queue is not empty...
                    while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                    {
                        // ...block RX thread to generate RX-Queue overflows
                        while (blockRxThread) Thread.Sleep(1000);

                        // ...receive data from hardware.
                        xlStatus = CANDriver.XL_CanReceive(portHandle, ref receivedEvent);

                        //  If receiving succeed....
                        if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                        {
                            rawRxString = CANDriver.XL_CanGetEventString(receivedEvent);
                        }
                    }
                }
                // No event occurred
            }
        }
    }
}
