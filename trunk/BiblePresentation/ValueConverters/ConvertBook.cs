using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace LiveBiblePresentation
{
    public class ConvertBook : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string book = (string)value + " ";
            return book;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
