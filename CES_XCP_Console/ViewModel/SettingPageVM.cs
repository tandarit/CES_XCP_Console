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
using Microsoft.Win32;

namespace CES_XCP_Console.ViewModel
{
    class SettingPageVM
    {        
        public MyICommand SaveCommand { get; set; }
        public MyICommand OpenA2lCommand { get; set; }
        public MyICommand OpenArXMLCommand { get; set; }

        public XCPEnviroment xcpEnv;

        public SettingPageVM()
        {
            ArbitDataRateList = new List<string>()
            {
                "20",
                "125",
                "500",
                "1000"
            };
            DataDataRateList = new List<string>()
            {
                "100",
                "1000",
                "2000",
                "8000"
            };

            xcpEnv = new XCPEnviroment();
           
            SaveCommand = new MyICommand(OnSave, CanSave);
            OpenA2lCommand = new MyICommand(OnOpenA2L, CanOpenA2L);
            OpenArXMLCommand = new MyICommand(OnOpenArXML, CanOpenArXML);
        }

        private void OnSave()
        {
            SaveEnviromentSettings("EnvConfig.xml");
            MessageBox.Show("Enviroment was saved!");
        }

        private bool CanSave()
        {
            return true;
        }

        private void OnOpenA2L()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();            
            openFileDialog.Filter= "A2L file (*.a2l)|*.a2l|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                xcpEnv.A2LFile = openFileDialog.FileName;
            }
        }

        private bool CanOpenA2L()
        {
            return true;
        }

        private void OnOpenArXML()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ArXML file(*.arxml)|*.arxml|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                xcpEnv.ArXMLFile = openFileDialog.FileName;
            }
        }

        private bool CanOpenArXML()
        {
            return true;
        }

        private void SaveEnviromentSettings(string fileName)
        {
            using (var stream=new FileStream(fileName, FileMode.Create))
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

        //List of DataBinding properies:
        
        public List<string> ArbitDataRateList
        {
            get;set;
        }

        public List<string> DataDataRateList
        {
            get;set;
        }

        public XCPEnviroment XCPEnviroment
        {
            get
            {
                return xcpEnv;
            }
            set
            {
                xcpEnv = value;
            }
        }
    }
}
