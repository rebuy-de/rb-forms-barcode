using System;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using Android.Views;
using Android.Hardware;
using AndroidCamera = Android.Hardware.Camera;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class ScannerCamera : ILog
    {
        private AndroidCamera camera;
        private static readonly ScannerCamera instance = new ScannerCamera();
        private readonly CameraConfigurator configurator = new CameraConfigurator();

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
            try {
                camera.SetPreviewDisplay(holder);
            } catch (Exception ex) {
                this.Debug(ex.Message);
                this.Debug(ex.StackTrace.ToString());
            }
        }

        public void ReleaseCamera()
        {
            this.Debug("ReleaseCamera");

            camera.Release();
            camera = null;
        }

        public void StartPreview(AndroidCamera.IPreviewCallback previewCallback) 
        {
            this.Debug("StartPreview");

            configurator.Configure(camera);

            camera.SetDisplayOrientation(90);
            camera.SetPreviewCallback(previewCallback);

            camera.StartPreview();
        }

        public void HaltPreview() 
        {
            this.Debug("HaltPreview");

            camera.CancelAutoFocus();

            camera.StopPreview();
            camera.SetPreviewCallback(null);
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