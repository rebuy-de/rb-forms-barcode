using System;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using Android.Views;
using Android.Hardware;
using AndroidCamera = Android.Hardware.Camera;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraControl : ILog
    {
        private AndroidCamera camera;
        private bool previewState = false;
        private static readonly CameraControl instance = new CameraControl();
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

        static CameraControl()
        {}

        private CameraControl()
        {}

        public static CameraControl Instance
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
                previewState = false;
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
            if (previewState) {
                return;
            }

            this.Debug("StartPreview");

            camera.StopPreview();

            configurator.Configure(camera);

            camera.SetDisplayOrientation(90);
            camera.SetPreviewCallback(previewCallback);

            camera.CancelAutoFocus();
            camera.StartPreview();

            previewState = true;
        }

        public void HaltPreview() 
        {
            if (!previewState) {
                return;
            }

            this.Debug("HaltPreview");

            camera.CancelAutoFocus();

            camera.StopPreview();
            camera.SetPreviewCallback(null);
            previewState = false;
        }

        public void AutoFocus(AndroidCamera.IAutoFocusCallback previewCallback)
        {
            if (CameraOpen) {
                camera.AutoFocus(previewCallback);
            }
        }
    }
}
#pragma warning restore 618