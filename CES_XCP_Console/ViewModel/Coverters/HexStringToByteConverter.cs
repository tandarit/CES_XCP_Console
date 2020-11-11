using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CES_XCP_Console.ViewModel.Coverters
{
    class HexStringToByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return String.Format("{0:X2}", (byte)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "0";
            }
            else
            {
                return byte.Parse((String)value, System.Globalization.NumberStyles.HexNumber);
            }
        }
    }
}
