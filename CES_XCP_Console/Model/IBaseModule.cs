using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_XCP_Console.Model  
{
    public interface IBaseModule
    {
        String Name {
            get;
            set;
        }
        String Description {
            get;
            set;
        }

    }
}
