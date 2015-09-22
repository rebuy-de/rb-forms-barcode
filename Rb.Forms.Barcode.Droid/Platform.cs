using System;
using Android.Content.PM;

namespace Rb.Forms.Barcode.Droid
{
    public class Platform
    {
        public bool HasCamera {
            get {
                return HasFeature(PackageManager.FeatureCamera);    
            }
        }

        public bool HasCameraPermission {
            get {
                return HasPermission(Android.Manifest.Permission.Camera);
            }
        }

        public bool HasFlash {
            get {
                return HasFeature(PackageManager.FeatureCameraFlash);    
            }
        }

        public bool HasFlashPermission {
            get {
                return HasPermission(Android.Manifest.Permission.Flashlight);
            }
        }

        public bool HasPermission(String permission)
        {
            return Permission.Granted == getPackageManager().CheckPermission(permission, getContext().PackageName);
        }

        public bool HasFeature(String feature)
        {
            return getPackageManager().HasSystemFeature(feature);
        }

        private Android.Content.Context getContext()
        {
            return Android.App.Application.Context;
        }

        private PackageManager getPackageManager()
        {
            return getContext().PackageManager;
        }
    }
}

