using System.ComponentModel;
using System.Drawing;
using System.Windows;

using LiveBiblePresentation.Resources;

namespace LiveBiblePresentation
{
    public class FrmLiveSettings : INotifyPropertyChanged
    {
        #region Public Properties

        public Color TextColor
        {
            get
            {
                return Settings.Default.TextColor;
            }
            set
            {
                Settings.Default.TextColor = value;
                OnPropertyChanged("TextColor");
            }
        }

        public Color ShadowColor
        {
            get
            {
                return Settings.Default.ShadowColor;
            }
            set
            {
                Settings.Default.ShadowColor = value;
                OnPropertyChanged("ShadowColor");
            }
        }

        public double FontSize
        {
            get
            {
                return Settings.Default.FontSize;
            }
            set
            {
                Settings.Default.FontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        public string BackgroundImagePath
        {
            get
            {
                return Settings.Default.BackgroundImagePath;
            }
            set
            {
                Settings.Default.BackgroundImagePath = value;
                OnPropertyChanged("BackgroundImagePath");
            }
        }

        public int NoOfVerses
        {
            get
            {
                return Settings.Default.NoOfVerses;
            }
            set
            {
                Settings.Default.NoOfVerses = value;
                OnPropertyChanged("NoOfVerses");
            }
        }

        public int DisplayNo
        {
            get
            {
                return Settings.Default.DisplayNo;
            }
            set
            {
                Settings.Default.DisplayNo = value;
                OnPropertyChanged("DisplayNo");
            }
        }

        public Visibility IsSettingsVisible
        {
            get
            {
                return Settings.Default.IsSettingsVisible;
            }
            set
            {
                Settings.Default.IsSettingsVisible = value;
                OnPropertyChanged("IsSettingsVisible");
            }
        }

        public Color SelectedTextColor
        {
            get
            {
                return Settings.Default.SelectedTextColor;
            }
            set
            {
                Settings.Default.SelectedTextColor = value;
                OnPropertyChanged("SelectedTextColor");
            }
        }

        public TextAlignment TextAlign
        {
            get
            {
                return Settings.Default.TextAlignment;
            }
            set
            {
                Settings.Default.TextAlignment = value;
                OnPropertyChanged("TextAlign");
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propName">Name of the prop.</param>
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
                Settings.Default.Save();
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}