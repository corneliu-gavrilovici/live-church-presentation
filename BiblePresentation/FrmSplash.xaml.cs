using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LiveBiblePresentation
{
	public partial class FrmSplash
	{
		public FrmSplash()
		{
			this.InitializeComponent();
            Loaded += new RoutedEventHandler(FrmSplash_Loaded);
            
		}

        void FrmSplash_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
	}
}