using Xamarin.Forms;
using Rb.Forms.Barcode.Pcl;
using System;

namespace Sample.Pcl.Pages
{
    public partial class ScannerPage : ContentPage
    {
        public ScannerPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);


            /**
             * So that we can release the camera when turning off phone or switching apps.
             */
            MessagingCenter.Subscribe<App>(this, App.MessageOnSleep, disableScanner);
            MessagingCenter.Subscribe<App>(this, App.MessageOnResume, enableScanner);

            barcodeScanner.BarcodeChanged += animateFlash;
        }

        protected override void OnAppearing()
        {
            barcodeScanner.PreviewActive = true;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.PreviewActive = false;
            base.OnDisappearing();
        }

        public void DisableScanner()
        {
            disableScanner(null);
        }

        private void gotoThirdPage(Object sender, EventArgs e)
        {
            Navigation.PushAsync(new ThirdPage());
        }


        /**
         * Release camera so that other apps can access it.
         */
        private void disableScanner(object sender)
        {
            barcodeScanner.IsEnabled = false;
        }

        /**
         * All your camera belongs to us.
         */
        private void enableScanner(object sender)
        {
            barcodeScanner.IsEnabled = true;
        }

        private async void animateFlash(object sender, BarcodeEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                await flash.FadeTo(1, 150, Easing.CubicInOut);
                flash.Opacity = 0;
            });      
        }

        /**
         * You need to take care of realeasing the camera when you are done with it else bad things can happen!
         */
        ~ScannerPage()
        {
            disableScanner(this);

            /**
             * Camera is released we dont need the events anymore.
             */
            MessagingCenter.Unsubscribe<App>(this, App.MessageOnSleep);
            MessagingCenter.Unsubscribe<App>(this, App.MessageOnResume);

            barcodeScanner.BarcodeChanged -= animateFlash;
        }
    }
}

