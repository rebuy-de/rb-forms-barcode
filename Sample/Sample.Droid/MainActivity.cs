using System;

using Android.App;
using Android.OS;
using Sample.Pcl;
using Xamarin.Forms.Platform.Android;
using Rb.Forms.Barcode.Droid;
using Android.Content.PM;
using System.Collections.Generic;

using AndroidCamera = Android.Hardware.Camera;

namespace Sample.Droid
{
    [Activity(Label = "Sample.Android", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Forms.Init(this, bundle);

            BarcodeScannerRenderer.Init();

            LoadApplication(new App());
        }
    }
}


