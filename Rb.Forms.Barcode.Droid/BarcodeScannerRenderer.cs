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
using System.Threading.Tasks;
using Rb.Forms.Barcode.Droid.View;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeScannerRenderer : ViewRenderer<BarcodeScanner, SurfaceView>, ISurfaceHolderCallback, ILog
    {
        private readonly ScannerCamera scannerCamera = ScannerCamera.Instance;
        private readonly BarcodeDecoder barcodeDecoder = new BarcodeDecoder();

        private PreviewFrameCallback previewFrameCallback;
        private CameraService scannerService;

        /// <summary>
        /// Checks the surface for validity so its safe to work with it.
        /// </summary>
        /// <value><c>true</c> if this instance has valid surface; otherwise, <c>false</c>.</value>
        private bool HasValidSurface {
            get {
                return Control.Holder.Surface.IsValid;
            }
        }

        private ViewStates Visibility { get; set; }

        /// <summary>
        /// Indicates that the currently open camera should be reused to prevent unexpected camera shutdowns.
        /// </summary>
        /// <value><c>true</c> if keep camera; otherwise, <c>false</c>.</value>
        private static bool KeepCamera { get; set; }

        public static void Init()
        {
        }

        public async void SurfaceCreated(ISurfaceHolder holder)
        {
            this.Debug("SurfaceCreated");

            if (!Element.IsEnabled) {
                return;
            }

            if (!HasValidSurface) {
                return;
            }

            if (scannerCamera.CameraOpen) {
                BarcodeScannerRenderer.KeepCamera = true;
                scannerService.HaltPreview();
            }

            await Task.Run(() => scannerService.OpenCamera(holder));
        }

        public void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
        {
            this.Debug("SurfaceChanged");

            if (!Element.IsEnabled) {
                return;
            }

            if (!scannerCamera.CameraOpen) {
                return;
            }

            if (!HasValidSurface) {
                return;
            }

            Element.PreviewActive = true;
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            this.Debug("SurfaceDestroyed");

            if (BarcodeScannerRenderer.KeepCamera) {
                return;
            }

            Element.IsEnabled = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BarcodeScanner> e)
        {
            this.Debug("OnElementChanged");

            base.OnElementChanged(e);

            if (Control != null) {
                return;
            }

            var surfaceView = new SurfaceView(Context);
            surfaceView.Holder.AddCallback(this);
            SetNativeControl(surfaceView);

            scannerService = new CameraService(Element, scannerCamera);
            previewFrameCallback = new PreviewFrameCallback(barcodeDecoder, Element);

            Element.CameraOpened += async (sender, args) => {
                if (Element.BarcodeDecoder) {
                    barcodeDecoder.RefreshToken();
                }

                if (Element.PreviewActive) {
                    await Task.Run(() => scannerService.StartPreview(previewFrameCallback));
                }
            };

            Element.CameraReleased += (sender, args) => {
                barcodeDecoder.CancelDecoding();
                BarcodeScannerRenderer.KeepCamera = false;
            };
        }

        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            this.Debug("OnElementPropertyChanged");

            if (e.PropertyName == BarcodeScanner.IsEnabledProperty.PropertyName) 
            {
                this.Debug("Enabled [{0}]", Element.IsEnabled);

                if (Element.IsEnabled && HasValidSurface) {
                    await Task.Run(() => scannerService.OpenCamera(Control.Holder));
                } 

                if (!Element.IsEnabled) {
                    scannerService.ReleaseCamera();
                }
            }

            if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) 
            {
                this.Debug("ScannerActive [{0}]", Element.PreviewActive);

                if (Element.PreviewActive) {
                    await Task.Run(() => scannerService.StartPreview(previewFrameCallback));
                } 

                if (!Element.PreviewActive) {
                    scannerService.HaltPreview();
                }
            }

            if (e.PropertyName == BarcodeScanner.BarcodeDecoderProperty.PropertyName) 
            {
                this.Debug("Decoder state [{0}]", Element.BarcodeDecoder);

                if (Element.BarcodeDecoder) {
                    barcodeDecoder.RefreshToken();
                } 

                if (!Element.BarcodeDecoder) {
                    barcodeDecoder.CancelDecoding();
                }
            }
        }

        protected async override void OnVisibilityChanged(global::Android.Views.View view, ViewStates visibility)
        {
            base.OnVisibilityChanged(view, visibility);

            this.Debug("OnVisibilityChanged [{0}]", visibility.ToString());

            if (Visibility == visibility) {
                return;
            }

            if (visibility == ViewStates.Visible) {
                await Task.Run(() => scannerService.StartPreview(previewFrameCallback));
            }

            if (visibility == ViewStates.Gone && !BarcodeScannerRenderer.KeepCamera) {
                scannerService.HaltPreview();
            }

            Visibility = visibility;
        }

        protected override void Dispose(bool disposing)
        {
            this.Debug("Disposing");

            if (disposing) {
                BarcodeScannerRenderer.KeepCamera = false;
            }

            base.Dispose(disposing);
        }
    }
}
