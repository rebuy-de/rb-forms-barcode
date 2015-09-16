using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.Logger;
using Android.Views;
using System.Threading.Tasks;
using Rb.Forms.Barcode.Droid.View;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeScannerRenderer : ViewRenderer<BarcodeScanner, SurfaceView>, ISurfaceHolderCallback, ILog
    {
        private static Configuration config;

        private CameraService cameraService;

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


        public static void Init()
        {
            BarcodeScannerRenderer.config = new Configuration();
        }

        public static void Init(Configuration config)
        {
            BarcodeScannerRenderer.config = config;
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

            await Task.Run(() => GetCameraService().OpenCamera(holder));
        }

        public void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
        {
            this.Debug("SurfaceChanged");
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            this.Debug("SurfaceDestroyed");

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

            Element.CameraOpened += async (sender, args) => {
                if (Element.PreviewActive) {
                    await Task.Run(() => GetCameraService().StartPreview(Control.Holder));
                }
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
                    await Task.Run(() => GetCameraService().OpenCamera(Control.Holder));
                } 

                if (!Element.IsEnabled) {
                    GetCameraService().ReleaseCamera();
                    cameraService = null;
                }
            }

            if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) 
            {
                this.Debug("ScannerActive [{0}]", Element.PreviewActive);

                if (Element.PreviewActive) {
                    await Task.Run(() => GetCameraService().StartPreview(Control.Holder));
                } 

                if (!Element.PreviewActive) {
                    GetCameraService().HaltPreview();
                }
            }
        }

        protected CameraService GetCameraService()
        {
            if (cameraService == null) {
                var configurator = new CameraConfigurator(config, Context);
                var factory = new CameraServiceFactory(configurator);
                cameraService = factory.Create(Context, Element);
            }

            return cameraService;
        }

        protected override void Dispose(bool disposing)
        {
            this.Debug("Disposing");

            cameraService = null;

            base.Dispose(disposing);
        }
    }
}
