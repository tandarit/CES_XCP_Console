using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_XCP_Console.Model
{
    class GROUP : IBaseModule, INotifyPropertyChanged
    {
        private List<string> refCharacteristicList;
        private String _name, _description;
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

        public List<string> RefCharacteristicList {
            get
            {
                return refCharacteristicList;
            }
            set
            {
                refCharacteristicList = value;
                RaisePropertyChanged("RefCharacteristicList");
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
