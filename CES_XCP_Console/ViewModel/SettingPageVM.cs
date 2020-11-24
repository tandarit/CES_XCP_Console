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
        public MyICommand OpenOwnDatabaseCommand { get; set; }

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

            if (MainWindowVM.sXcpEnv != null)
                xcpEnv = MainWindowVM.sXcpEnv;
            else
                xcpEnv = new XCPEnviroment();
           
            SaveCommand = new MyICommand(OnSave, CanSave);
            OpenA2lCommand = new MyICommand(OnOpenA2L, CanOpenA2L);
            OpenArXMLCommand = new MyICommand(OnOpenArXML, CanOpenArXML);
            OpenOwnDatabaseCommand = new MyICommand(OnOpenOwnDatabase, CanOpenOwnDatabase);
        }

        private void OnSave()
        {
            String fileName;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = ".xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                fileName = saveFileDialog1.FileName;
                using (var myStream = new FileStream(fileName, FileMode.Create))
                {
                        XmlSerializer xml = new XmlSerializer(typeof(XCPEnviroment));
                        xml.Serialize(myStream, xcpEnv);
                        myStream.Close();

                        //Save to registry
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\CES_XCP_Console");
                        key.SetValue("ConfigFilePath", fileName);
                        key.Close();
                }                    
            }

            //SaveEnviromentSettings();
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

        private void OnOpenOwnDatabase()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML file(*.xml)|*.xml|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                xcpEnv.OwnDatabaseFile = openFileDialog.FileName;
            }
        }

        private bool CanOpenOwnDatabase()
        {
            //TODO
            return true;
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
