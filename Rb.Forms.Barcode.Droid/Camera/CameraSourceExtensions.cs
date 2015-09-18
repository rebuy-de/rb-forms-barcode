using System;
using Android.Gms.Vision;
using AndroidCamera = Android.Hardware.Camera;
using Rb.Forms.Barcode.Droid.View;
using System.Linq;


namespace Rb.Forms.Barcode.Droid.Camera
{
    public static class CameraSourceExtensions
    {
        public static AndroidCamera GetCamera(this CameraSource cameraSource)
        {
            var fields = cameraSource.Class.GetDeclaredFields();
            var cameraClass = Java.Lang.Class.FromType(typeof(AndroidCamera));

            var field = fields.FirstOrDefault(f => f.Type == cameraClass);
            field.Accessible = true;

            return field.Get(cameraSource) as AndroidCamera;
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

