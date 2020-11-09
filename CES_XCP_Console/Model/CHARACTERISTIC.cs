using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_XCP_Console.Model
{
    class CHARACTERISTIC : IBaseModule, INotifyPropertyChanged   {
        private String _name, _description;
        private UInt32 address;
        private String variableType, conversionType;
        private bool isreadonly;
        private decimal arraysize;

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

        public UInt32 Address {
            get
            {
                return address;
            }
            set
            {
                address = value;
                RaisePropertyChanged("Address");
            } 
        } //VALUE or VAL_BLK
        public String VariableType {
            get
            {
                return variableType;
            }
            set
            {
                variableType = value;
                RaisePropertyChanged("VariableType");
            } 
        }        
        public String ConversionType {
            get
            {
                return conversionType;
            }
            set
            {
                conversionType = value;
                RaisePropertyChanged("ConversionType");
            }
        }
        public bool isReadOnly {
            get
            {
                return isreadonly;
            }
            set
            {
                isreadonly = value;
                RaisePropertyChanged("isReadOnly");
            }
        }
        public decimal ArraySize { 
            get
            {
                return arraysize;
            }
            set
            {
                arraysize = value;
                RaisePropertyChanged("ArraySize");
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
