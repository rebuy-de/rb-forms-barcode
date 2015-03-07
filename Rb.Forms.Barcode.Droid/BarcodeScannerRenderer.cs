using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.Decoder;
using Rb.Forms.Barcode.Droid.Logger;

using Android.Views;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeScannerRenderer : ViewRenderer<BarcodeScanner, SurfaceView>, ISurfaceHolderCallback, ILog
    {
        private readonly CameraControl cameraControl = CameraControl.Instance;
        private readonly BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
        private ViewStates currentVisibility;
        private static bool reuseCamera;

        private AutoFocusCallback autoFocus;
        private PreviewFrameCallback previewFrameCallback;

        public static void Init()
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            this.Debug("SurfaceCreated");

            if (cameraControl.CameraOpen) {
                BarcodeScannerRenderer.reuseCamera = true;
                haltPreview();
            }

            openCamera();
        }

        public void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
        {
            this.Debug("SurfaceChanged");

            try {
                if (currentVisibility != ViewStates.Gone) {
                    startPreview();
                }
            } catch (Exception ex) {
                this.Debug("Unable to start preview.");
                this.Debug(ex.ToString());
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            this.Debug("SurfaceDestroyed");

            if (!BarcodeScannerRenderer.reuseCamera) {
                haltPreview();
                releaseCamera();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BarcodeScanner> e)
        {
            this.Debug("OnElementChanged");

            base.OnElementChanged(e);

            if (Control == null) {
                var surfaceView = new SurfaceView(Context);
                surfaceView.Holder.AddCallback(this);
                SetNativeControl(surfaceView);

                autoFocus = new AutoFocusCallback(cameraControl);
                previewFrameCallback = new PreviewFrameCallback(barcodeDecoder, Element);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            this.Debug("OnElementPropertyChanged");

            if (e.PropertyName == BarcodeScanner.IsEnabledProperty.PropertyName) 
            {
                this.Debug("Enabled [{0}]", Element.IsEnabled);

                if (Element.IsEnabled) {
                    openCamera();
                    startPreview();
                } 

                if (!Element.IsEnabled) {
                    haltPreview();
                    releaseCamera();
                }
            }
        }

        protected override void OnVisibilityChanged(global::Android.Views.View view, ViewStates visibility)
        {
            base.OnVisibilityChanged(view, visibility);

            this.Debug("OnVisibilityChanged [{0}]", visibility.ToString());

            if (currentVisibility == visibility) {
                return;
            }

            if (visibility == ViewStates.Visible) {
                startPreview();
            }

            if (visibility == ViewStates.Gone && !BarcodeScannerRenderer.reuseCamera) {
                haltPreview();
            }

            currentVisibility = visibility;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                this.Debug("Disposing");
                BarcodeScannerRenderer.reuseCamera = false;
            }

            base.Dispose(disposing);
        }

        private void openCamera()
        {
            /**
             * When switching apps the surface seems not to be valid.
             * Simply do nothing and let the follow up event fix it.
             */
            if (!Control.Holder.Surface.IsValid) {
                return;
            }

            barcodeDecoder.RefreshToken();

            try {
                cameraControl.OpenCamera();
                cameraControl.AssignPreview(Control.Holder);
            } catch (Exception ex) {
                this.Debug("Unable to open camera");
                this.Debug(ex.ToString());
            }
        }

        private void releaseCamera()
        {
            try {
                barcodeDecoder.CancelDecoding();
                cameraControl.ReleaseCamera();
                autoFocus.Enabled = false;

                BarcodeScannerRenderer.reuseCamera = false;
            } catch (Exception ex) {
                this.Debug("Unable to release camera");
                this.Debug(ex.ToString());
            }
        }

        private void startPreview()
        {
            /**
             * When switching apps the surface seems not to be valid.
             * Simply do nothing and let the follow up event fix it.
             */
            if (!Control.Holder.Surface.IsValid) {
                return;
            }

            cameraControl.StartPreview(previewFrameCallback);

            if (cameraControl.AutoFocusMode) {
                cameraControl.AutoFocus(autoFocus);
            }
        }

        private void haltPreview()
        {
            autoFocus.Enabled = false;
            cameraControl.HaltPreview();
        }
    }
}
