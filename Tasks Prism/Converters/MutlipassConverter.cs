using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Tasks_Prism.Converters
{
    public class MutlipassConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var passwordBoxes = new List<PasswordBox>();
            foreach (var v in values)
            {
                passwordBoxes.Add((PasswordBox)v);
            }
            return passwordBoxes;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
