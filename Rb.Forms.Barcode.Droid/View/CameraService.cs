using System;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.Logger;
using Android.Views;
using Xamarin.Forms;

namespace Rb.Forms.Barcode.Droid.View
{
    public class CameraService : ILog
    {
        private readonly BarcodeScanner renderer;

        private readonly ScannerCamera scannerCamera;
        private readonly AutoFocusCallback autoFocus;

        public CameraService(BarcodeScanner renderer, ScannerCamera scannerCamera)
        {
            this.renderer = renderer;
            this.scannerCamera = scannerCamera;
            this.autoFocus = new AutoFocusCallback(scannerCamera);
        }

        public void OpenCamera(ISurfaceHolder holder)
        {
            try {
                scannerCamera.OpenCamera();
                scannerCamera.AssignPreview(holder);

                renderer.OnCameraOpened();

                renderer.PreviewActive = true;
            } catch (Exception ex) {
                this.Debug("Unable to open camera");
                this.Debug(ex.ToString());
            }
        }

        public void ReleaseCamera()
        {
            autoFocus.Enabled = false;

            renderer.PreviewActive = false;

            try {
                scannerCamera.ReleaseCamera();

                renderer.OnCameraReleased();
            } catch (Exception ex) {
                this.Debug("Unable to release camera");
                this.Debug(ex.ToString());
            }

        }

        public void StartPreview(PreviewFrameCallback previewFrameCallback)
        {
            try {
                scannerCamera.StartPreview(previewFrameCallback);

                if (scannerCamera.AutoFocusMode) {
                    scannerCamera.AutoFocus(autoFocus);
                }

                renderer.OnPreviewActivated();

            } catch (Exception ex) {
                this.Debug("Unable to start preview.");
                this.Debug(ex.ToString());
            }
        }

        public void HaltPreview()
        {
            autoFocus.Enabled = false;
            scannerCamera.HaltPreview();

            renderer.OnPreviewDeactivated();
        }
    }
}

