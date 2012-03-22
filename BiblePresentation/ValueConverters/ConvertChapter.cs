using System;
using System.Windows.Data;

namespace LiveBiblePresentation
{
    public class ConvertChapter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string chapter = value.ToString() + ":";
            return chapter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
