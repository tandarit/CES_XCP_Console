using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CES_XCP_Console.VectorCAN;
using Microsoft.Win32;
using CES_XCP_Console.Model;

namespace CES_XCP_Console.ViewModel
{
    class MainWindowVM
    {
        private VectorCan vectorCan;
        public static XCPEnviroment sXcpEnv;

        public MainWindowVM()
        {
            vectorCan = new VectorCan();
            vectorCan.OpenDriver();
        }

        public String GetVectorCanInfo
        {
            get
            {
                return vectorCan.Info;
            }
        }

    }
}
