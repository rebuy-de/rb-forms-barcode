using System;

using Android.App;
using Android.OS;
using Sample.Pcl;
using Xamarin.Forms.Platform.Android;
using Rb.Forms.Barcode.Droid;
using Android.Content.PM;
using Xamarin.Forms;
using AndroidCamera = Android.Hardware.Camera;
using System.Collections.Generic;

namespace Sample.Droid
{
    [Activity(Label = "Sample.Android", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            var config = new Configuration {
                // Some devices, mostly samsung, stop auto focusing as soon as one of the advanced features is enabled.
                CompatibilityMode = Build.Manufacturer.Contains("samsung"),
                Zoom = 5
            };

            BarcodeScannerRenderer.Init(config);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
