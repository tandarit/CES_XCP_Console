using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using CES_XCP_Console.Model;

namespace CES_XCP_Console.ViewModel
{
    class SettingPageVM
    {
        
        public MyICommand SaveCommand { get; set; }

        public SettingPageVM()
        {
            ArbitDataRateList = new ObservableCollection<string>()
            {
                "20",
                "125",
                "500",
                "1000"
            };
            DataDataRateList = new ObservableCollection<string>()
            {
                "100",
                "1000",
                "2000",
                "8000"
            };
           
            SaveCommand = new MyICommand(OnSave, CanSave);
        }

        private void OnSave()
        {
            
            SaveEnviromentSettings("EnvConfig.xml");
        }

        private bool CanSave()
        {

            return true;
        }

        private void SaveEnviromentSettings(string fileName)
        {
            XCPEnviroment xcpEnv = new XCPEnviroment();


            using(var stream=new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer xml = new XmlSerializer(typeof(XCPEnviroment));
                xml.Serialize(stream, xcpEnv);
                stream.Close();
            }
        }

        private XCPEnviroment LoadEnviromentSettings(string fileName) {
            XCPEnviroment xcpEnv;
            using (var stream = new FileStream("EnvConf.xml", FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(XCPEnviroment));
                xcpEnv=(XCPEnviroment)xml.Deserialize(stream);
                stream.Close();
            }
            return xcpEnv;

        }

        //List of bitrate
        
        public ObservableCollection<string> ArbitDataRateList
        {
            get;set;
        }

        public ObservableCollection<string> DataDataRateList
        {
            get;set;
        }

        private byte[] nmDummyContent;

        public String Name
        {
            get;
            set;
        }

        public bool IsCanFd
        {
            get;
            set;
        }
        public uint ArbitrationBitrate
        {
            get;
            set;
        }
        public uint DataBitrate
        {
            get;
            set;
        }
        public String A2LFile
        {
            get;
            set;
        }
        public String ArXMLFile
        {
            get;
            set;
        }
        public uint NMDummyID
        {
            get;
            set;
        }
       
        public byte RepeatTime
        {
            get;
            set;
        }
        public uint XCPReq
        {
            get;
            set;
        }
        public uint XCPRes
        {
            get;
            set;
        }
    }
}
