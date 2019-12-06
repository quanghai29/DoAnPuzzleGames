using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace _1712384_1712349_1712407
{
    class RelativeToAbsolutePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string relative = (string)value;
            string absolute = "Images/" + relative;
                //$"{AppDomain.CurrentDomain.BaseDirectory}Images\\{relative}";
            //Debug.WriteLine(absolute);
            return absolute;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
