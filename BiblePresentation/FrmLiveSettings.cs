using LiveBiblePresentation.Resources;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

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

        public System.Windows.Media.Color ShadowColor
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

        public double ShadowOpacity
        {
            get
            {
                return Settings.Default.ShadowOpacity;
            }
            set
            {
                Settings.Default.ShadowOpacity = value;
                OnPropertyChanged("ShadowOpacity");
            }
        }

        public double ShadowBlurRadius
        {
            get
            {
                return Settings.Default.ShadowBlurRadius;
            }
            set
            {
                Settings.Default.ShadowBlurRadius = value;
                OnPropertyChanged("ShadowBlurRadius");
            }
        }

        public double ShadowDirection
        {
            get
            {
                return Settings.Default.ShadowDirection;
            }
            set
            {
                Settings.Default.ShadowDirection = value;
                OnPropertyChanged("ShadowDirection");
            }
        }

        public double ShadowDepth
        {
            get
            {
                return Settings.Default.ShadowDepth;
            }
            set
            {
                Settings.Default.ShadowDepth = value;
                OnPropertyChanged("ShadowDepth");
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

        public System.Windows.Media.FontFamily FontFamily
        {
            get
            {
                return new System.Windows.Media.FontFamily(Settings.Default.FontFamily);
            }
            set
            {
                Settings.Default.FontFamily = value.Source;
                OnPropertyChanged("FontFamily");
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