using System;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.Logger;
using Android.Views;
using Android.Gms.Vision;

namespace Rb.Forms.Barcode.Droid.View
{
    public class CameraService : ILog
    {
        private readonly BarcodeScanner renderer;
        private readonly CameraSource cameraSource;

        private readonly AutoFocusCallback autoFocus;
        private readonly CameraConfigurator cameraConfigurator;

        private bool started = false;
        private bool configured = false;

        public CameraService(BarcodeScanner renderer, CameraSource cameraSource, CameraConfigurator configurator)
        {
            this.renderer = renderer;
            this.cameraSource = cameraSource;
            this.cameraConfigurator = configurator;

            this.autoFocus = new AutoFocusCallback(cameraSource.GetCamera());
        }

        public void OpenCamera(ISurfaceHolder holder)
        {
            try {
                if (started) {
                    cameraSource.Stop();
                }

                cameraSource.Start(holder);
                started = true;

                renderer.OnCameraOpened();
            } catch (Exception ex) {
                this.Debug("Unable to open camera");
                this.Debug(ex.ToString());
            }
        }

        public void ReleaseCamera()
        {
            autoFocus.Enabled = false;

            try {
                cameraSource?.Release();
                started = false;
                configured = false;

                renderer.OnCameraReleased();
            } catch (Exception ex) {
                this.Debug("Unable to release camera");
                this.Debug(ex.ToString());
            }

        }

        public void StartPreview(ISurfaceHolder holder)
        {
            try {
                if (!started) {
                    OpenCamera(holder);
                    return;
                }

                if (!configured) {
                    cameraConfigurator.Configure(cameraSource);

                    if (cameraSource.AutoFocusModeEnabled()) {
                        cameraSource.AutoFocus(autoFocus);
                    }

                    configured = true;

                    renderer.OnPreviewActivated();
                }
            } catch (Exception ex) {
                this.Debug("Unable to start preview.");
                this.Debug(ex.ToString());
            }
        }

        public void HaltPreview()
        {
            try {
                autoFocus.Enabled = false;
                cameraSource?.Stop();
                started = false;
                configured = false;

                renderer.OnPreviewDeactivated();

            } catch (Exception ex) {
                this.Debug("Unable to halt preview.");
                this.Debug(ex.ToString());
            }

        }
    }
}

