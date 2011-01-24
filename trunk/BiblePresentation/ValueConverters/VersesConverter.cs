using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using LiveBiblePresentation.Data;

namespace LiveBiblePresentation.ValueConverters
{
    public class VersesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = string.Empty;
            foreach (BibleVerse verse in (BibleVerses)value)
            {
                text = text + verse.Carte + "\n" + verse.Capitol.ToString() + ":" + verse.Verset.ToString() + " " + verse.Text + "\n";
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
