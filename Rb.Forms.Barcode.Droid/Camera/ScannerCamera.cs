using System;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using Android.Views;
using Android.Hardware;
using AndroidCamera = Android.Hardware.Camera;
using ApxLabs.FastAndroidCamera;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class ScannerCamera : ILog
    {
        private AndroidCamera camera;
        private static readonly ScannerCamera instance = new ScannerCamera();

        public bool CameraOpen {
            get {
                return camera != null;
            }
        }

        public bool AutoFocusMode {
            get {
                return camera.GetParameters().FocusMode == AndroidCamera.Parameters.FocusModeAuto;
            }
        }

        static ScannerCamera()
        {}

        private ScannerCamera()
        {}

        public static ScannerCamera Instance
        {
            get
            {
                return instance;
            }
        }

        public AndroidCamera OpenCamera()
        {
            this.Debug("OpenCamera");

            if (camera == null) {
                camera = AndroidCamera.Open();
            }

            return camera;
        }

        public void AssignPreview(ISurfaceHolder holder)
        {
            this.Debug("AssignPreview");

            camera.SetPreviewDisplay(holder);
        }

        public void ReleaseCamera()
        {
            this.Debug("ReleaseCamera");

            camera.Release();
            camera = null;
        }

        public void StartPreview(INonMarshalingPreviewCallback previewCallback) 
        {
            this.Debug("StartPreview");

            camera.SetNonMarshalingPreviewCallback(previewCallback);

            camera.StartPreview();
        }

        public void HaltPreview() 
        {
            this.Debug("HaltPreview");

            camera.CancelAutoFocus();

            camera.StopPreview();
            camera.SetNonMarshalingPreviewCallback(null);
        }

        public void AutoFocus(AndroidCamera.IAutoFocusCallback previewCallback)
        {
            camera.CancelAutoFocus();

            if (CameraOpen) {
                camera.AutoFocus(previewCallback);
            }
        }
    }
}
#pragma warning restore 618