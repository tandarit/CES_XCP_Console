using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CES_XCP_Console.VectorCAN;

namespace CES_XCP_Console.ViewModel
{
    class MainWindowVM
    {
        private VectorCan vectorCan;

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
