using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Sample.Pcl.Pages
{
    public partial class MainPage : MasterDetailPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void gotoScannerPage(Object sender, EventArgs e)
        {
            Detail.Navigation.PushAndRemovePrevious(ScannerPageControl.Instance.CreateScannerPage(), 1);
            IsPresented = false;
        }

        private void gotoMainPage(Object sender, EventArgs e)
        {
            Detail.Navigation.PopToRootAsync();
            IsPresented = false;
        }
    }
}

