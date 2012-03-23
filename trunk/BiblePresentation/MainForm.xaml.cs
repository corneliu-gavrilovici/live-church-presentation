using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using LiveBiblePresentation.Data;
using LiveBiblePresentation.Resources;

namespace LiveBiblePresentation
{
    public partial class MainForm
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            Hide();
            splash = new FrmSplash();
            splash.Show();
            this.InitializeComponent();

            try
            {
                frmLiveSettings = new FrmLiveSettings();
                DataContext = frmLiveSettings;
                bdOptions.Visibility = frmLiveSettings.IsSettingsVisible;
                richTextBox.Document.TextAlignment = frmLiveSettings.TextAlign;

                Width = Settings.Default.MainFormSize.Width;
                Height = Settings.Default.MainFormSize.Height;
                txtText.Document.Blocks.Clear();
                txtText.Document.Blocks.Add(new Paragraph(new Run(Settings.Default.Text.Trim())));

                InitializeNotifyIcon();

                m_manager = new BibleManager(Settings.Default.BibleLanguage);
                List<string> biblleBooks = m_manager.GetBibleBooks();
                foreach (string book in biblleBooks)
                {
                    cbxBooks.Items.Add(book);
                }

                VerseID = Settings.Default.VerseID;
                cbxBooks.Text = biblleBooks[0];
                cbxBooks.Focus();

                PopulateSettingsControls();
                EventManager.RegisterClassHandler(typeof(System.Windows.Controls.Button), System.Windows.Controls.Button.ClickEvent, new RoutedEventHandler(Button_Click));
                System.Windows.Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

                splash.Close();

                tabControl.SelectionChanged += tabControl_SelectionChanged;
                radLeft.Checked += radtextAlign_CheckedChanged;
                radRight.Checked += radtextAlign_CheckedChanged;
                radCenter.Checked += radtextAlign_CheckedChanged;

                cbxChapters.DropDownOpened += cbxChapters_DropDownOpened;
                cbxVerses.DropDownOpened += cbxVerses_DropDownOpened;
                richTextBox.TextChanged += richTextBox_TextChanged;
                richTextBox.SelectionChanged += richTextBox_SelectionChanged;
                txtText.SelectionChanged += richTextBox_SelectionChanged;
                foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
                {
                    FontsComboBox.Items.Add(fontFamily.Source);
                    FontsComboBox1.Items.Add(fontFamily.Source);
                }

                Show();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                Close();
            }
        }

        #endregion

        #region Private Event Handlers

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyContentLive();
        }

        private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.RichTextBox)sender).Name == richTextBox.Name)
            {
                FontFamily style = (FontFamily)richTextBox.Selection.GetPropertyValue(FontFamilyProperty);
                FontsComboBox.SelectedItem = style.Source;
            }
            else
            {
                FontFamily style = (FontFamily)txtText.Selection.GetPropertyValue(FontFamilyProperty);
                FontsComboBox1.SelectedItem = style.Source;
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CopyContentLive();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show(e.Exception.Message);
        }

        private void cbxVerses_DropDownOpened(object sender, EventArgs e)
        {
            int lastVerseSelIndex = cbxVerses.SelectedIndex;
            cbxVerses.Items.Clear();
            int noOfVerses = m_manager.GetNoOfVerses((string)cbxBooks.SelectedValue, (int)cbxChapters.SelectedValue);
            for (int i = 1; i <= noOfVerses; i++)
            {
                cbxVerses.Items.Add(i);
            }

            cbxVerses.SelectedIndex = lastVerseSelIndex;
            if (cbxVerses.SelectedItem == null && cbxVerses.Items.Count > 0)
                cbxVerses.SelectedIndex = 0;
        }

        private void cbxChapters_DropDownOpened(object sender, EventArgs e)
        {
            int lastSelChapterIndex = cbxChapters.SelectedIndex;
            int lastVerseSelIndex = cbxVerses.SelectedIndex;

            cbxChapters.Items.Clear();
            int bookChapters = m_manager.GetBookChapters((string)cbxBooks.SelectedValue);
            for (int i = 1; i <= bookChapters; i++)
            {
                cbxChapters.Items.Add(i);
            }

            cbxChapters.SelectedIndex = lastSelChapterIndex;
            cbxVerses.SelectedIndex = lastVerseSelIndex;

            if (cbxVerses.SelectedItem == null && cbxVerses.Items.Count > 0)
                cbxVerses.SelectedIndex = 0;
            if (cbxChapters.SelectedItem == null && cbxChapters.Items.Count > 0)
                cbxChapters.SelectedIndex = 0;
        }

        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox senderCombo = (System.Windows.Controls.ComboBox)sender;
            txtSearch.Clear();
            try
            {
                switch (senderCombo.Name)
                {
                    case "cbxBooks":
                        cbxChapters.Items.Clear();
                        int bookChapters = m_manager.GetBookChapters((string)cbxBooks.SelectedValue);
                        for (int i = 1; i <= bookChapters; i++)
                        {
                            cbxChapters.Items.Add(i);
                        }
                        cbxChapters.SelectedIndex = 0;
                        break;
                    case "cbxChapters":
                        cbxVerses.Items.Clear();
                        int noOfVerses = m_manager.GetNoOfVerses((string)cbxBooks.SelectedValue, (int)cbxChapters.SelectedValue);
                        for (int i = 1; i <= noOfVerses; i++)
                        {
                            cbxVerses.Items.Add(i);
                        }
                        cbxVerses.SelectedIndex = 0;
                        break;
                }

                VerseID = m_manager.GetID((string)cbxBooks.SelectedValue, (int)cbxChapters.SelectedValue, (int)cbxVerses.SelectedValue);
                BibleVerses verses = m_manager.GetVerses(VerseID, NoOfVerses + 1);
                PopulateWithVerses(verses);
            }
            catch
            {
                richTextBox.Document.Blocks.Clear();
            }
        }

        private void lbName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Path_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnforward_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetNext();
        }

        private void btnback_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetPrevious();
        }

        private void btnGO_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DisplayLiveForm(frmLiveSettings.DisplayNo - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                System.Windows.MessageBox.Show("You cannot 'GO LIVE' when you are in search mode! \n          Click the verse you want to display.", "Live Presentation");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void frmLive_Closed(object sender, EventArgs e)
        {
            frmLive = null;
            if (m_storyBoard != null)
                m_storyBoard.Stop(this);
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Input.MouseDevice mouseDevice = System.Windows.Input.Mouse.PrimaryDevice;
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control &&
                e.Key == System.Windows.Input.Key.L)
            {
                btnGO_MouseUp(sender, new System.Windows.Input.MouseButtonEventArgs(mouseDevice, 0, System.Windows.Input.MouseButton.Left));
            }

            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control &&
                e.Key == Key.F)
            {
                txtSearch.Focus();
            }
        }

        private void m_notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
            }
            Settings.Default.MainFormSize = new System.Drawing.Size(Convert.ToInt16(Width),
                                                                    Convert.ToInt16(Height));
            frmLiveSettings.IsSettingsVisible = bdOptions.Visibility;
            Settings.Default.Text = new TextRange(txtText.Document.ContentStart, txtText.Document.ContentEnd).Text;
            Settings.Default.Save();
            System.Windows.Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;

                switch (button.Name)
                {
                    case "btnSearch":
                        Search();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void FontsComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.ComboBox)sender).Name == FontsComboBox.Name)
            {
                richTextBox.Selection.ApplyPropertyValue(FontFamilyProperty, FontsComboBox.SelectedValue);
            }
            else
            {
                txtText.Selection.ApplyPropertyValue(FontFamilyProperty, FontsComboBox1.SelectedValue);
            }
        }

        #region Options Event Handlers

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "JPEG Compressed Image (*.jpg)|*.jpg|Windows Bitmap (*.bmp)|*.bmp| Portable Network Graphics (*.png) |*.png | Graphics Interchange Format (*.gif)| *.gif";
            if (openFileDialog.ShowDialog().Value == true)
            {
                frmLiveSettings.BackgroundImagePath = openFileDialog.FileName;
                if (frmLive != null)
                {
                    frmLive.imgBack.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                }
            }
        }

        private void btnTextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = frmLiveSettings.TextColor;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                frmLiveSettings.TextColor = colorDialog.Color;
                if (frmLive != null)
                {
                    ((FrmLiveSettings)frmLive.DataContext).TextColor = colorDialog.Color;
                }
            }
        }

        private void btnSelectedTextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = frmLiveSettings.SelectedTextColor;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                frmLiveSettings.SelectedTextColor = colorDialog.Color;
                if (frmLive != null)
                {
                    ((FrmLiveSettings)frmLive.DataContext).SelectedTextColor = colorDialog.Color;
                    frmLive.richTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.RichTextBox.ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)));
                    frmLive.richTextBox.Focus();
                }
            }
        }

        private void cbxNoOfVerses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            frmLiveSettings.NoOfVerses = Convert.ToInt16(cbxNoOfVerses.SelectedItem.ToString());
            BibleVerses verses = m_manager.GetVerses(VerseID, NoOfVerses + 1);
            PopulateWithVerses(verses);
        }

        private void cbxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            frmLiveSettings.FontSize = Convert.ToDouble(cbxFontSize.SelectedItem.ToString());
        }

        private void cbxDisplays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            frmLiveSettings.DisplayNo = Convert.ToInt16(cbxDisplays.SelectedItem.ToString());
            if (frmLive != null)
                DisplayLiveForm(frmLiveSettings.DisplayNo);
        }

        private void btnPositionSize_Click(object sender, RoutedEventArgs e)
        {
            if (frmLive != null)
            {
                switch (((RepeatButton)sender).Name)
                {
                    case "btnDown":
                        frmLive.Top++;
                        break;
                    case "btnUp":
                        frmLive.Top--;
                        break;
                    case "btnLeft":
                        frmLive.Left--;
                        break;
                    case "btnRight":
                        frmLive.Left++;
                        break;
                    case "btnZoomWMinus":
                        frmLive.Width -= 50;
                        // frmLive.Height -= 30;
                        break;
                    case "btnZoomWPlus":
                        frmLive.Width += 50;
                        // frmLive.Height += 30;
                        break;
                    case "btnZoomHMinus":
                        //frmLive.Width -= 50;
                        frmLive.Height -= 30;
                        break;
                    case "btnZoomHPlus":
                        //frmLive.Width += 50;
                        frmLive.Height += 30;
                        break;
                }
            }
            else
            {
                System.Windows.MessageBox.Show(Settings.Default.GoLive, this.Title, System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            frmLiveSettings.BackgroundImagePath = string.Empty;
            if (frmLive != null)
            {
                frmLive.imgBack.Source = null;
            }
        }

        #endregion

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets the verse ID.
        /// </summary>
        /// <value>The verse ID.</value>
        private int VerseID
        {
            get
            {
                return (int)stackPanel.Tag;
            }
            set
            {
                stackPanel.Tag = value;
            }
        }

        /// <summary>
        /// Gets the no of verses.
        /// </summary>
        /// <value>The no of verses.</value>
        private int NoOfVerses
        {
            get
            {
                return Settings.Default.NoOfVerses;
            }
        }

        #endregion

        #region Private Methods

        private void CopyContentLive()
        {
            if (frmLive != null)
            {
                TextRange mainRange = null;
                if (tabControl.SelectedItem == tabItemBible)
                {
                    if (richTextBox.Document.Blocks.LastBlock == null)
                        return;
                    mainRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.Blocks.LastBlock.ElementStart);
                }
                else
                {
                    mainRange = new TextRange(txtText.Document.ContentStart, txtText.Document.ContentEnd);
                }

                Stream stream = new MemoryStream();
                mainRange.Save(stream, System.Windows.DataFormats.Xaml, true);

                TextRange liveRange = new TextRange(frmLive.richTextBox.Document.ContentStart, frmLive.richTextBox.Document.ContentEnd);
                liveRange.Load(stream, System.Windows.DataFormats.Xaml);
                stream.Close();
            }
        }

        private void Search()
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
                return;

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            BibleVerses verses = m_manager.Search(txtSearch.Text);
            PopulateWithVerses(verses);

            if (txtSearch.ToolTip != null)
            {
                ((System.Windows.Controls.ToolTip)txtSearch.ToolTip).IsOpen = false;
            }
            System.Windows.Controls.ToolTip toolTip = new System.Windows.Controls.ToolTip();
            toolTip.Background = Background;
            toolTip.BorderBrush = BorderBrush;
            toolTip.Foreground = Brushes.White;
            toolTip.Content = "Found in " + verses.Count.ToString() + " verses";
            toolTip.IsOpen = true;
            toolTip.PlacementTarget = txtSearch;
            toolTip.Placement = PlacementMode.Bottom;
            toolTip.StaysOpen = false;
            txtSearch.ToolTip = toolTip;

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();

            //Add ContextMenu.
            System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            exitItem.Click += exitItem_Click;
            menu.Items.Add(exitItem);

            notifyIcon.ContextMenuStrip = menu;
            notifyIcon.Visible = true;
            notifyIcon.Text = "Live Bible Presentation";
            notifyIcon.Icon = Resource.greenbook;
            notifyIcon.DoubleClick += m_notifyIcon_DoubleClick;
        }

        private void GetPrevious()
        {
            if (VerseID == 1) return;

            txtSearch.Clear();
            VerseID = VerseID - NoOfVerses;
            BibleVerses bibleVerses = m_manager.GetVerses(VerseID, NoOfVerses + 1);
            PopulateWithVerses(bibleVerses);

            Populating(bibleVerses);
        }

        private void GetNext()
        {
            if (VerseID == 31102) return;

            txtSearch.Clear();
            VerseID = VerseID + NoOfVerses;
            BibleVerses bibleVerses = m_manager.GetVerses(VerseID, NoOfVerses + 1);
            PopulateWithVerses(bibleVerses);

            Populating(bibleVerses);
        }

        private void DisplayLiveForm(int displayNo)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            //Close the form if already open.
            if ((frmLive != null) && (m_storyBoard != null))
            {
                frmLive.Close();
                frmLive = null;
                m_storyBoard.Stop(this);
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                return;
            }

            try
            {
                frmLive = new FrmLive();
                frmLive.Closed += frmLive_Closed;
                frmLive.DataContext = frmLiveSettings;

                if (!String.IsNullOrEmpty(frmLiveSettings.BackgroundImagePath))
                {
                    frmLive.imgBack.Source = new BitmapImage(new Uri(frmLiveSettings.BackgroundImagePath, UriKind.Absolute));
                }

                if (tabItemBible.IsSelected)
                {
                    frmLive.m_spaceKeyPressed += GetNext;
                    frmLive.m_backSpaceKeyPressed += GetPrevious;
                    BibleVerses bibleVerses = m_manager.GetVerses(VerseID, NoOfVerses);
                    CopyContentLive();

                }
                if (tabItemText.IsSelected)
                {
                    if (txtText.Document.Blocks.Count == 0)
                    {
                        System.Windows.MessageBox.Show("No text to display!");
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                        return;
                    }
                    CopyContentLive();
                }

                if (frmLiveSettings.DisplayNo == 1)
                {
                    frmLive.Topmost = true;
                    frmLive.WindowState = WindowState.Maximized;
                }
                else
                {
                    Screen screen = (Screen)Screen.AllScreens.GetValue(displayNo);
                    try
                    {
                        if (screen.Bounds.Contains((int)Settings.Default.FrmLiveLeft, (int)Settings.Default.FrmLiveTop))
                        {
                            frmLive.Width = Settings.Default.FrmLiveWidth;
                            frmLive.Height = Settings.Default.FrmLiveHeight;
                            frmLive.Top = Settings.Default.FrmLiveTop;
                            frmLive.Left = Settings.Default.FrmLiveLeft;
                        }
                        else
                        {
                            frmLive.Top = Convert.ToDouble(screen.Bounds.Top);
                            frmLive.Left = Convert.ToDouble(screen.Bounds.Left);
                        }
                    }
                    catch (NullReferenceException)
                    {
                        frmLive.Top = Convert.ToDouble(screen.Bounds.Top);
                        frmLive.Left = Convert.ToDouble(screen.Bounds.Left);
                    }
                }

                frmLive.Show();

                m_storyBoard = ((Storyboard)Resources["OnBibleLive"]);
                m_storyBoard.RepeatBehavior = RepeatBehavior.Forever;
                BeginStoryboard(m_storyBoard, HandoffBehavior.SnapshotAndReplace, true);
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }
            catch (IndexOutOfRangeException)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                System.Windows.MessageBox.Show("Your computer does not have the selected display activated!", this.Title, System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Populating(BibleVerses bibleVerse)
        {
            cbxVerses.SelectionChanged -= cbx_SelectionChanged;
            cbxBooks.SelectionChanged -= cbx_SelectionChanged;
            cbxChapters.SelectionChanged -= cbx_SelectionChanged;
            cbxBooks.Text = bibleVerse[bibleVerse.Count - NoOfVerses - 1].Carte;
            cbxChapters.Text = bibleVerse[bibleVerse.Count - NoOfVerses - 1].Capitol.ToString();
            cbxVerses.Text = bibleVerse[bibleVerse.Count - NoOfVerses - 1].Verset.ToString();
            cbxVerses.SelectionChanged += cbx_SelectionChanged;
            cbxBooks.SelectionChanged += cbx_SelectionChanged;
            cbxChapters.SelectionChanged += cbx_SelectionChanged;
        }

        private void PopulateWithVerses(BibleVerses verses)
        {
            richTextBox.Document.Blocks.Clear();

            List<string> books = new List<string>();
            foreach (BibleVerse verse in verses)
            {
                Paragraph verseParagraph = new Paragraph();

                if (verses.IndexOf(verse) == verses.Count - 1)
                {
                    verseParagraph.Foreground = Brushes.Orange;
                    verseParagraph.Inlines.Add(new LineBreak());
                }

                if (!books.Contains(verse.Carte))
                {
                    Run bookName = new Run(verse.Carte);
                    bookName.FontWeight = FontWeights.UltraBold;
                    verseParagraph.Inlines.Add(bookName);

                    verseParagraph.Inlines.Add(new LineBreak());
                    verseParagraph.Inlines.Add(new LineBreak());
                    books.Add(verse.Carte);
                }

                Run chapterVerseName = new Run(verse.Capitol.ToString() + ":" + verse.Verset.ToString());


                chapterVerseName.MouseLeave += chapterVerseName_MouseLeave;
                chapterVerseName.MouseDown += chapterVerseName_MouseDown;
                chapterVerseName.MouseEnter += chapterVerseName_MouseEnter;
                chapterVerseName.Cursor = System.Windows.Input.Cursors.Hand;
                chapterVerseName.Tag = verse.ID;

                chapterVerseName.FontWeight = FontWeights.ExtraBold;
                verseParagraph.Inlines.Add(chapterVerseName);

                if (!String.IsNullOrEmpty(txtSearch.Text) && !goToVerseInProcess)
                {
                    Run leftRun = new Run(" " + verse.Text.Substring(0, verse.Text.ToLower().IndexOf(txtSearch.Text.ToLower())));
                    Run foundText = new Run(verse.Text.Substring(verse.Text.ToLower().IndexOf(txtSearch.Text.ToLower()), txtSearch.Text.Length));
                    foundText.Foreground = Brushes.SaddleBrown;
                    foundText.FontWeight = FontWeights.UltraBlack;
                    Run rightRun = new Run(verse.Text.Substring(verse.Text.ToLower().LastIndexOf(txtSearch.Text.ToLower()) + txtSearch.Text.Length));

                    verseParagraph.Inlines.Add(leftRun);
                    verseParagraph.Inlines.Add(foundText);
                    verseParagraph.Inlines.Add(rightRun);
                }
                else
                {
                    Run verseText = new Run(" " + verse.Text.Trim());
                    verseParagraph.Inlines.Add(verseText);
                }

                richTextBox.Document.Blocks.Add(verseParagraph);
            }
        }

        private void chapterVerseName_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Run)sender).TextDecorations = null;
        }

        private void chapterVerseName_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Run)sender).TextDecorations = TextDecorations.Underline;
        }

        private void chapterVerseName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            goToVerseInProcess = true;
            VerseID = Convert.ToInt16(((Run)sender).Tag);
            BibleVerses verses = m_manager.GetVerses(Convert.ToInt16(((Run)sender).Tag), NoOfVerses + 1);
            PopulateWithVerses(verses);

            txtSearch.Clear();
            goToVerseInProcess = false;
        }

        private void PopulateSettingsControls()
        {
            cbxBibleTextLanguage.ItemsSource = Enum.GetValues(typeof(BibleLanguage));
            cbxBibleTextLanguage.SelectedValue = Settings.Default.BibleLanguage;
            cbxBibleTextLanguage.SelectionChanged += cbxBibleTextLanguage_SelectionChanged;

            //Populate FontSize Combo.
            for (int i = 25; i <= 70; i++)
            {
                cbxFontSize.Items.Add(i.ToString());
            }

            // Populate NoOfSscreens Combo.
            for (int i = 1; i <= SystemInformation.MonitorCount; i++)
            {
                cbxDisplays.Items.Add(i.ToString());
            }

            // Populate NoOfVerses Combo.
            for (int i = 1; i <= 6; i++)
            {
                cbxNoOfVerses.Items.Add(i.ToString());
            }

            cbxDisplays.Text = frmLiveSettings.DisplayNo.ToString();
            cbxFontSize.Text = frmLiveSettings.FontSize.ToString();
            cbxNoOfVerses.Text = frmLiveSettings.NoOfVerses.ToString();

            cbxNoOfVerses.SelectionChanged += cbxNoOfVerses_SelectionChanged;
            cbxDisplays.SelectionChanged += cbxDisplays_SelectionChanged;
            cbxFontSize.SelectionChanged += cbxFontSize_SelectionChanged;
            btnTextColor.Click += btnTextColor_Click;
            btnImage.Click += btnImage_Click;

            btnLeft.Click += btnPositionSize_Click;
            btnUp.Click += btnPositionSize_Click;
            btnRight.Click += btnPositionSize_Click;
            btnDown.Click += btnPositionSize_Click;
            btnZoomWMinus.Click += btnPositionSize_Click;
            btnZoomWPlus.Click += btnPositionSize_Click;
            btnZoomHMinus.Click += btnPositionSize_Click;
            btnZoomHPlus.Click += btnPositionSize_Click;

            switch (frmLiveSettings.TextAlign)
            {
                case TextAlignment.Center:
                    radCenter.IsChecked = true;
                    break;
                case TextAlignment.Left:
                    radLeft.IsChecked = true;
                    break;
                case TextAlignment.Right:
                    radRight.IsChecked = true;
                    break;
            }
        }

        private void cbxBibleTextLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Settings.Default.BibleLanguage = (BibleLanguage)cbxBibleTextLanguage.SelectedValue;
            Settings.Default.Save();
            m_manager = new BibleManager(Settings.Default.BibleLanguage);

            List<string> biblleBooks = m_manager.GetBibleBooks();

            int prevSelBooKIndex = cbxBooks.SelectedIndex;
            int prevSelChapterIndex = cbxChapters.SelectedIndex;
            int prevSelVerseIndex = cbxVerses.SelectedIndex;

            cbxBooks.Items.Clear();
            foreach (string book in biblleBooks)
            {
                cbxBooks.Items.Add(book);
            }
            cbxBooks.SelectedIndex = prevSelBooKIndex;
            cbxChapters.SelectedIndex = prevSelChapterIndex;
            cbxVerses.SelectedIndex = prevSelVerseIndex;

            BibleVerses verses = m_manager.GetVerses(VerseID, NoOfVerses + 1);
            PopulateWithVerses(verses);

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void radtextAlign_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            if (radCenter.IsChecked == true)
            {
                richTextBox.Document.TextAlignment = TextAlignment.Center;
            }
            else if (radLeft.IsChecked == true)
            {
                richTextBox.Document.TextAlignment = TextAlignment.Left;
            }
            else
            {
                richTextBox.Document.TextAlignment = TextAlignment.Right;
            }

            frmLiveSettings.TextAlign = richTextBox.Document.TextAlignment;

            if (frmLive != null)
            {
                frmLive.richTextBox.Document.TextAlignment = richTextBox.Document.TextAlignment;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnColor1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RichTextBox rich = (((System.Windows.Controls.Button)sender).Name == btnColor1.Name) ? richTextBox : txtText;
            FrmLiveSettings settings = (FrmLiveSettings)DataContext;
            if (rich.Selection.GetPropertyValue(ForegroundProperty).ToString() == new SolidColorBrush(System.Windows.Media.Color.FromArgb(settings.SelectedTextColor.A, settings.SelectedTextColor.R, settings.SelectedTextColor.G, settings.SelectedTextColor.B)).ToString())
            {
                rich.Selection.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(settings.TextColor.A, settings.TextColor.R, settings.TextColor.G, settings.TextColor.B)));
            }
            else
            {

                rich.Selection.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(settings.SelectedTextColor.A, settings.SelectedTextColor.R, settings.SelectedTextColor.G, settings.SelectedTextColor.B)));
            }
        }

        #endregion

        #region Private Members

        private BibleManager m_manager = null;
        private FrmLive frmLive = null;
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private Storyboard m_storyBoard = null;
        private FrmLiveSettings frmLiveSettings = null;
        private FrmSplash splash = null;
        private bool goToVerseInProcess = false;

        #endregion
    }
}