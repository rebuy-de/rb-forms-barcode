using System;
using System.Drawing;
using Android.Views;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.Logger;
using Rb.Forms.Barcode.Droid.View;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Pcl.Logger;
using RebuyCameraSource = Com.Rebuy.Play.Services.Vision.CameraSource;

namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraService : ILog
    {
        private readonly BarcodeScanner renderer;
        private readonly Com.Rebuy.Play.Services.Vision.CameraSource cameraSource;

        private readonly AutoFocusCallback autoFocus;
        private readonly CameraConfigurator cameraConfigurator;

        private bool opened = false;
        private bool previewActive = false;

        public CameraService(BarcodeScanner renderer, RebuyCameraSource cameraSource, CameraConfigurator configurator)
        {
            this.renderer = renderer;
            this.cameraSource = cameraSource;
            this.cameraConfigurator = configurator;

            this.autoFocus = new AutoFocusCallback(cameraSource.Camera);
        }

        public void OpenCamera()
        {
            try {
                if (opened) {
                    cameraSource.Stop();
                }

                cameraSource.Open();
                opened = true;

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
                opened = false;
                previewActive = false;

                renderer.OnCameraReleased();
            } catch (Exception ex) {
                this.Debug("Unable to release camera");
                this.Debug(ex.ToString());
            }

        }

        public void StartPreview(ISurfaceHolder holder)
        {
            try {
                if (!opened) {
                    OpenCamera();
                    return;
                }

                if (!previewActive) {
                    cameraSource?.StartPreview(holder);

                    if (cameraSource.AutoFocusModeEnabled()) {
                        cameraSource.AutoFocus(autoFocus);
                    }

                    previewActive = true;
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
                cameraSource?.StopPreview();
                previewActive = false;

                renderer.OnPreviewDeactivated();

            } catch (Exception ex) {
                this.Debug("Unable to halt preview.");
                this.Debug(ex.ToString());
            }
        }

        public void StartDecoder()
        {
            cameraSource?.StartFrameProcessor();
        }

        public void StopDecoder()
        {
            cameraSource?.StopFrameProcessor();
        }

        public void ToggleTorch(bool state) 
        {
            cameraConfigurator.ToggleTorch(cameraSource?.Camera, state);
        }

        public void SetViewSize(int width, int height)
        {
            cameraConfigurator.SetViewSize(new Size(width, height));
        }
    }
}

