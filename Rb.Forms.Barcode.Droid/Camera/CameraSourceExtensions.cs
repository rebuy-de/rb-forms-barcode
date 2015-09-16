using System;
using Android.Gms.Vision;
using AndroidCamera = Android.Hardware.Camera;
using Rb.Forms.Barcode.Droid.View;

namespace Rb.Forms.Barcode.Droid.Camera
{
    public static class CameraSourceExtensions
    {
        public static AndroidCamera GetCamera(this CameraSource cameraSource)
        {
            var fields = cameraSource.Class.GetDeclaredFields();

            foreach (var field in fields) {
                if (field.Type.CanonicalName == "android.hardware.Camera") {
                    field.Accessible = true;

                    return (AndroidCamera) field.Get(cameraSource);
                }
            }

            return null;
        }

        public static bool AutoFocusModeEnabled(this CameraSource cameraSource)
        {
            return cameraSource.GetCamera()?.GetParameters().FocusMode == AndroidCamera.Parameters.FocusModeAuto;
        }

        public static void AutoFocus(this CameraSource cameraSource, AutoFocusCallback callback)
        {
            cameraSource.GetCamera()?.AutoFocus(callback);
        }
    }
}

