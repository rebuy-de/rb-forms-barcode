using System;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.Logger;
using Android.Views;

namespace Rb.Forms.Barcode.Droid.View
{
    public class CameraService : ILog
    {
        private readonly BarcodeScanner renderer;

        private readonly ScannerCamera scannerCamera;
        private readonly AutoFocusCallback autoFocus;
        private readonly CameraConfigurator cameraConfigurator;

        public CameraService(BarcodeScanner renderer, ScannerCamera scannerCamera, CameraConfigurator configurator)
        {
            this.renderer = renderer;
            this.scannerCamera = scannerCamera;
            this.cameraConfigurator = configurator;

            this.autoFocus = new AutoFocusCallback(scannerCamera);
        }

        public void OpenCamera(ISurfaceHolder holder)
        {
            try {
                var camera = scannerCamera.OpenCamera();

                scannerCamera.AssignPreview(holder);

                renderer.OnCameraOpened();
            } catch (Exception ex) {
                this.Debug("Unable to open camera");
                this.Debug(ex.ToString());
            }
        }

        public void ReleaseCamera()
        {
            autoFocus.Enabled = false;

            HaltPreview();

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
                cameraConfigurator.Configure(scannerCamera.OpenCamera());
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
            try {
                autoFocus.Enabled = false;
                scannerCamera.HaltPreview();

                renderer.OnPreviewDeactivated();

            } catch (Exception ex) {
                this.Debug("Unable to halt preview.");
                this.Debug(ex.ToString());
            }

        }
    }
}

