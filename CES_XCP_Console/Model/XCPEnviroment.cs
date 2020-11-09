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
        public byte[] NMDummyContent
        {
            get { return nmDummyContent; }
            set { nmDummyContent = value; }
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
