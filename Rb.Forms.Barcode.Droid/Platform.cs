using System;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;

namespace Rb.Forms.Barcode.Droid
{
    public class Platform
    {
        /// <summary>
        /// Helper property to check if the device has a camera.
        /// </summary>
        public bool HasCamera {
            get {
                return HasFeature(PackageManager.FeatureCamera);    
            }
        }

        /// <summary>
        /// Helper property to check if app/lib is allowed to access the camera.
        /// </summary>
        public bool HasCameraPermission {
            get {
                return HasPermission(Android.Manifest.Permission.Camera);
            }
        }

        /// <summary>
        /// Helper property to check if the device has a flash.
        /// </summary>
        public bool HasFlash {
            get {
                return HasFeature(PackageManager.FeatureCameraFlash);    
            }
        }

        /// <summary>
        /// Helper property to check if app/lib is allowed to access the flash.
        /// </summary>
        public bool HasFlashPermission {
            get {
                return HasPermission(Android.Manifest.Permission.Flashlight);
            }
        }

        /// <summary>
        /// Checks if the google mobile services are installed and ready to use.
        /// </summary>
        public bool IsGmsReady {
            get {
                return new BarcodeDetector.Builder(getContext()).Build().IsOperational;
            }
        }

        /// <summary>
        /// Checks if the app/library has access to the asked permission.
        /// </summary>
        public bool HasPermission(String permission)
        {
            return Permission.Granted == getPackageManager().CheckPermission(permission, getContext().PackageName);
        }

        /// <summary>
        /// Checks if the device has the asked feature.
        /// </summary>
        public bool HasFeature(String feature)
        {
            return getPackageManager().HasSystemFeature(feature);
        }

        private Context getContext()
        {
            return Android.App.Application.Context;
        }

        private PackageManager getPackageManager()
        {
            return getContext().PackageManager;
        }
    }
}

