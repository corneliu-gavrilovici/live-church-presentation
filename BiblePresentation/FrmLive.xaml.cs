using LiveBiblePresentation.Resources;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LiveBiblePresentation
{
    public delegate void SpaceKeyPressed();
    public delegate void BackSpaceKeyPressed();

    public partial class FrmLive
    {
        #region Public Methods

        public FrmLive()
        {
            this.InitializeComponent();

            PreviewKeyDown += FrmLive_PreviewKeyDown;
            Closing += FrmLive_Closing;
            Loaded += FrmLive_Loaded;
        }

        public void SetShadowSettings()
        {
            FrmLiveSettings liveSettings = DataContext as FrmLiveSettings;
            if (liveSettings != null)
            {
                dropShadowText.Color = liveSettings.ShadowColor;
                dropShadowText.Opacity = liveSettings.ShadowOpacity;
                dropShadowText.BlurRadius = liveSettings.ShadowBlurRadius;
                dropShadowText.Direction = liveSettings.ShadowDirection;
                dropShadowText.ShadowDepth = liveSettings.ShadowDepth;
            }
        }

        #endregion

        #region Private Event Handlers

        private void FrmLive_Loaded(object sender, RoutedEventArgs e)
        {
            SetShadowSettings();
        }

        private void FrmLive_Closing(object sender, CancelEventArgs e)
        {
            if ((DataContext as FrmLiveSettings).DisplayNo > 1)
            {
                Settings.Default.FrmLiveWidth = Width;
                Settings.Default.FrmLiveHeight = Height;
                Settings.Default.FrmLiveTop = Top;
                Settings.Default.FrmLiveLeft = Left;
                Settings.Default.Save();
            }
        }

        private void FrmLive_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                Close();

            if (e.Key == System.Windows.Input.Key.Space || e.Key == System.Windows.Input.Key.Right || e.Key == System.Windows.Input.Key.Enter)
            {
                if (m_spaceKeyPressed != null)
                    m_spaceKeyPressed();
            }

            if (e.Key == System.Windows.Input.Key.Back || e.Key == System.Windows.Input.Key.Left)
            {
                if (m_backSpaceKeyPressed != null)
                    m_backSpaceKeyPressed();
            }
        }

        private void btnTextDecorations_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;

            switch (buttonName)
            {
                case "btnBold":
                    if (richTextBox.Selection.GetPropertyValue(RichTextBox.FontWeightProperty).ToString() == "ExtraBold")
                    {
                        richTextBox.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.Normal);
                    }
                    else
                    {
                        richTextBox.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.UltraBold);
                    }

                    break;
                case "btnItalic":
                    if (richTextBox.Selection.GetPropertyValue(FontStyleProperty).ToString() == "Oblique")
                    {
                        richTextBox.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Normal);
                    }
                    else
                    {
                        richTextBox.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Oblique);
                    }

                    break;
                case "btnUnderline":
                    if (richTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) == TextDecorations.Underline)
                    {
                        richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                    }
                    else
                    {
                        richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                    }
                    break;
                case "btnColor":
                    System.Drawing.Color color = ((FrmLiveSettings)DataContext).SelectedTextColor;
                    if (richTextBox.Selection.GetPropertyValue(RichTextBox.ForegroundProperty).ToString() == new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)).ToString())
                    {
                        richTextBox.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, Brushes.White);
                    }
                    else
                    {
                        richTextBox.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)));
                    }
                    break;
            }

            richTextBox.Focus();
        }

        #endregion

        #region Private Members

        public SpaceKeyPressed m_spaceKeyPressed = null;
        public BackSpaceKeyPressed m_backSpaceKeyPressed = null;

        #endregion
    }
}