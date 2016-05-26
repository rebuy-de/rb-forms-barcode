using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Sample.Pcl.Pages
{
    public partial class RootPage : ContentPage
    {
        public RootPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, true);
        }

        private async void gotoScannerPage(Object sender, EventArgs e)
        {
            var cameraPermission = new CameraPermission(CrossPermissions.Current);
            await cameraPermission.RequestCameraPermissionIfNeeded();
            Navigation.PushAndRemovePrevious(ScannerPageControl.Instance.CreateScannerPage(), 1);
        }
    }
}

