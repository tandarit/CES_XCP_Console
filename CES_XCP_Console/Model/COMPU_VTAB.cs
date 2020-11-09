using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_XCP_Console.Model
{
    class COMPU_VTAB : IBaseModule, INotifyPropertyChanged
    {
        private String _name, _description;
        private int count;
        private Dictionary<ulong, string> instance;

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public String Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }

        }

        public int Count {
            get
            {
                return count;
            }
            set
            {
                count = value;
                RaisePropertyChanged("Count");
            }
        }
        public Dictionary<ulong, string> Instance {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
                RaisePropertyChanged("Instance");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
