using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Sample.Pcl.Pages
{
    public partial class RootPage : ContentPage
    {
        public RootPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, true);
        }

        private void gotoScannerPage(Object sender, EventArgs e)
        {
            Navigation.PushAndRemovePrevious(ScannerPageControl.Instance.CreateScannerPage(), 1);
        }
    }
}

