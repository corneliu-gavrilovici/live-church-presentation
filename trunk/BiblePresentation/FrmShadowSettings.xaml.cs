using System.Windows;
using System.Windows.Media;

namespace LiveBiblePresentation
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmShadowSettings : Window
    {
        private FrmLive frmLive = null;
        private readonly FrmLiveSettings frmLiveSettings = null;

        public FrmShadowSettings(FrmLive frmLive, FrmLiveSettings frmLiveSettings)
        {
            InitializeComponent();

            this.frmLive = frmLive;
            this.frmLiveSettings = frmLiveSettings;
            btnOK.IsDefault = true;
            btnCancel.IsCancel = true;

            btnColor.SelectedColor = frmLiveSettings.ShadowColor;
            txtOpacity.Value = frmLiveSettings.ShadowOpacity;
            txtBlurRadius.Value = frmLiveSettings.ShadowBlurRadius;
            txtDirection.Value = frmLiveSettings.ShadowDirection;
            txtShadowDepth.Value = frmLiveSettings.ShadowDepth;
             
            txtOpacity.ValueChanged += txtOpacity_ValueChanged;
            txtBlurRadius.ValueChanged += txtBlurRadius_ValueChanged;
            txtShadowDepth.ValueChanged += txtShadowDepth_ValueChanged;
            txtDirection.ValueChanged += txtDirection_ValueChanged;
            btnColor.SelectedColorChanged += btnColor_SelectedColorChanged;
            btnCancel.Click += btnCancel_Click;
            btnOK.Click += btnOK_Click;
            btnReset.Click += btnReset_Click;
        }

        public FrmLive FrmLive
        {
            set { frmLive = value; }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            btnColor.SelectedColor = Colors.WhiteSmoke;
            txtOpacity.Value = 0.5;
            txtBlurRadius.Value = 5;
            txtDirection.Value = 315;
            txtShadowDepth.Value = 4;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            frmLiveSettings.ShadowColor = btnColor.SelectedColor;
            if (txtOpacity.Value.HasValue)
                frmLiveSettings.ShadowOpacity = txtOpacity.Value.Value;
            if (txtBlurRadius.Value.HasValue)
                frmLiveSettings.ShadowBlurRadius = txtBlurRadius.Value.Value;
            if (txtDirection.Value.HasValue)
                frmLiveSettings.ShadowDirection = txtDirection.Value.Value;
            if (txtShadowDepth.Value.HasValue)
                frmLiveSettings.ShadowDepth = txtShadowDepth.Value.Value;

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (frmLive != null)
                frmLive.SetShadowSettings();

            Close();
        }

        private void btnColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (frmLive != null)
            {
                frmLive.dropShadowText.Color = btnColor.SelectedColor;
            }
        }

        private void txtDirection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (frmLive != null)
            {
                frmLive.dropShadowText.Direction = txtDirection.Value.Value;
            }
        }

        private void txtShadowDepth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (frmLive != null)
            {
                frmLive.dropShadowText.ShadowDepth = txtShadowDepth.Value.Value;
            }
        }

        private void txtBlurRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (frmLive != null)
            {
                frmLive.dropShadowText.BlurRadius = txtBlurRadius.Value.Value;
            }
        }

        private void txtOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (frmLive != null)
            {
                frmLive.dropShadowText.Opacity = txtOpacity.Value.Value;
            }
        }
    }
}