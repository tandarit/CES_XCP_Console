using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_XCP_Console.Model
{
    public class XCPEnviroment :INotifyPropertyChanged
    {
        private String _Name;
        private bool _IsCanFd;
        private uint _ArbitrationBitrate;
        private uint _DataBitrate;
        private String _A2LFile;
        private String _ArXMLFile;
        private uint _NMDummyID;
        private byte _NMDummyContent0;
        private byte _NMDummyContent1;
        private byte _NMDummyContent2;
        private byte _NMDummyContent3;
        private byte _NMDummyContent4;
        private byte _NMDummyContent5;
        private byte _NMDummyContent6;
        private byte _NMDummyContent7;
        private uint _RepeatTime;
        private uint _XCPReq;
        private uint _XCPRes;
        private string _OwnDatabaseFile;


        public String Name
        {
            get {
                return _Name;
            }
            set
            {
                if (_Name != value)
                    _Name = value;
                else
                    RaisePropertyChanged("Name");
            }
        }

        public bool IsCanFd
        {
            get
            {
                return _IsCanFd;
            }
            set
            {
                if (_IsCanFd != value)
                    _IsCanFd = value;
                else
                    RaisePropertyChanged("IsCanFd");
            }
        }
        public uint ArbitrationBitrate
        {
            get
            {
                return _ArbitrationBitrate;
            }
            set
            {
                if (_ArbitrationBitrate != value)
                    _ArbitrationBitrate = value;
                else
                    RaisePropertyChanged("ArbitrationBitrate");
            }
        }
        public uint DataBitrate
        {
            get
            {
                return _DataBitrate;
            }
            set
            {
                if (_DataBitrate != value)
                    _DataBitrate = value;
                else
                    RaisePropertyChanged("DataBitrate");
            }
        }
        public String A2LFile
        {
            get
            {
                return _A2LFile;
            }
            set
            {
                if (_A2LFile != value)
                    _A2LFile = value;
                else
                    RaisePropertyChanged("A2LFile");
            }
        }
        public String ArXMLFile
        {
            get
            {
                return _ArXMLFile;
            }
            set
            {
                if (_ArXMLFile != value)
                    _ArXMLFile = value;
                else
                    RaisePropertyChanged("ArXMLFile");
            }
        }
        public uint NMDummyID
        {
            get
            {
                return _NMDummyID;
            }
            set
            {
                if (_NMDummyID != value)
                    _NMDummyID = value;
                else
                    RaisePropertyChanged("NMDummyID");
            }
        }
        public byte NMDummyContent0
        {
            get
            {
                return _NMDummyContent0;
            }
            set
            {
                if (_NMDummyContent0 != value)
                    _NMDummyContent0 = value;
                else
                    RaisePropertyChanged("NMDummyContent0");
            }
        }
        public byte NMDummyContent1
        {
            get
            {
                return _NMDummyContent1;
            }
            set
            {
                if (_NMDummyContent1 != value)
                    _NMDummyContent1 = value;
                else
                    RaisePropertyChanged("NMDummyContent1");
            }
        }
        public byte NMDummyContent2
        {
            get
            {
                return _NMDummyContent2;
            }
            set
            {
                if (_NMDummyContent2 != value)
                    _NMDummyContent2 = value;
                else
                    RaisePropertyChanged("NMDummyContent2");
            }
        }
        public byte NMDummyContent3
        {
            get
            {
                return _NMDummyContent3;
            }
            set
            {
                if (_NMDummyContent3 != value)
                    _NMDummyContent3 = value;
                else
                    RaisePropertyChanged("NMDummyContent3");
            }
        }
        public byte NMDummyContent4
        {
            get
            {
                return _NMDummyContent4;
            }
            set
            {
                if (_NMDummyContent4 != value)
                    _NMDummyContent4 = value;
                else
                    RaisePropertyChanged("NMDummyContent4");
            }
        }
        public byte NMDummyContent5
        {
            get
            {
                return _NMDummyContent5;
            }
            set
            {
                if (_NMDummyContent5 != value)
                    _NMDummyContent5 = value;
                else
                    RaisePropertyChanged("NMDummyContent5");
            }
        }
        public byte NMDummyContent6
        {
            get
            {
                return _NMDummyContent6;
            }
            set
            {
                if (_NMDummyContent6 != value)
                    _NMDummyContent6 = value;
                else
                    RaisePropertyChanged("NMDummyContent6");
            }
        }
        public byte NMDummyContent7
        {
            get
            {
                return _NMDummyContent7;
            }
            set
            {
                if (_NMDummyContent7 != value)
                    _NMDummyContent7 = value;
                else
                    RaisePropertyChanged("NMDummyContent7");
            }
        }

        public uint RepeatTime
        {
            get
            {
                return _RepeatTime;
            }
            set
            {
                if (_RepeatTime != value)
                    _RepeatTime = value;
                else
                    RaisePropertyChanged("RepeatTime");
            }
        }
        public uint XCPReq
        {
            get
            {
                return _XCPReq;
            }
            set
            {
                if (_XCPReq != value)
                    _XCPReq = value;
                else
                    RaisePropertyChanged("XCPReq");
            }
        }
        public uint XCPRes
        {
            get
            {
                return _XCPRes;
            }
            set
            {
                if (_XCPRes != value)
                    _XCPRes = value;
                else
                    RaisePropertyChanged("XCPRes");
            }
        }

        public String OwnDatabaseFile
        {
            get
            {
                return _OwnDatabaseFile;
            }
            set
            {
                if (_OwnDatabaseFile != value)
                    _OwnDatabaseFile = value;
                else
                    RaisePropertyChanged("XCPRes");
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
