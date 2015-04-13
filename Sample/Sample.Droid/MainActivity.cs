using System;

using Android.App;
using Android.OS;
using Sample.Pcl;
using Xamarin.Forms.Platform.Android;
using Rb.Forms.Barcode.Droid;
using Android.Content.PM;
using Xamarin.Forms;

namespace Sample.Droid
{
    [Activity(Label = "Sample.Android", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            var config = new RbConfig {
                Metrics = true
            };

            BarcodeScannerRenderer.Init(config);

            LoadApplication(new App());
        }
    }
}


