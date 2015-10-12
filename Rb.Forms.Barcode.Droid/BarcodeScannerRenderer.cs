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
using Android.Content.PM;
using Android.Runtime;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeScannerRenderer : ViewRenderer<BarcodeScanner, SurfaceView>, ISurfaceHolderCallback, ILog
    {
        /// <summary>
        /// The desired BarcodeScannerRenderer configuration.
        /// </summary>
        /// <see cref="Configuration" />
        private static Configuration config;

        /// <summary>
        /// Camera configuration callback. 
        /// We want to keep the same instance over the life time of the BarcodeScannerRenderer so that we 
        /// remember configured options when Enabling/Disabling the camera
        /// </summary>
        private readonly CameraConfigurator configurator;

        /// <summary>
        /// Do not use this. Use the Property instead.
        /// </summary>
        /// <see cref="BarcodeScannerRenderer.CameraService" />
        private CameraService cameraServiceHolder;

        private CameraService CameraService {
            get {
                if (null == cameraServiceHolder) {
                    var factory = new CameraServiceFactory();
                    cameraServiceHolder = factory.Create(Context, Element, configurator);                
                }

                return cameraServiceHolder;
            }
        }

        private Lazy<Platform> platform;

        private Platform Platform {
            get {
                return platform.Value;
            }
        }

        /// <summary>
        /// Checks the surface for validity so its safe to work with it.
        /// </summary>
        /// <value><c>true</c> if this instance has valid surface; otherwise, <c>false</c>.</value>
        private bool HasValidSurface {
            get {
                return Control?.Holder.Surface.IsValid == true;
            }
        }

        public static void Init()
        {
            Init(new Configuration());
        }

        public static void Init(Configuration config)
        {
            BarcodeScannerRenderer.config = config;
        }

        public BarcodeScannerRenderer()
        {
            configurator = new CameraConfigurator().SetConfiguration(config);
            platform = new Lazy<Platform>(() => new Platform());
        }

        public async void SurfaceCreated(ISurfaceHolder holder)
        {
            this.Debug("SurfaceCreated");
        }

        public async void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
        {
            this.Debug("SurfaceChanged");

            if (!HasValidSurface) {
                return;
            }

            // portrait mode
            if (height > width) {
                CameraService.SetViewSize(height, width);
            } else {
                CameraService.SetViewSize(width, height);
            }

            if (!Element.IsEnabled) {
                return;
            }

            await Task.Run(() => {
                CameraService.HaltPreview();
                CameraService.StartPreview(holder);
            });
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

            if (!Platform.HasCameraPermission) {
                this.Debug("Unable to setup scanner: Android Manifest '{0}' permission not granted.", Android.Manifest.Permission.Camera);
                return;
            }

            if (!Platform.HasCamera) {
                this.Debug("Unable to setup scanner: Device has no camera.");
                return;
            }

            if (!Platform.IsGmsReady) {
                this.Debug("Unable to setup scanner: Google mobile services are not ready.");
                return;
            }

            if (Control != null) {
                return;
            }

            var surfaceView = new SurfaceView(Context);
            surfaceView.Holder.AddCallback(this);
            SetNativeControl(surfaceView);

            Element.CameraOpened += async (sender, args) => {
                if (Element.PreviewActive) {
                    await Task.Run(() => CameraService.StartPreview(Control.Holder));
                }
            };
        }

        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            this.Debug("OnElementPropertyChanged");

            if (!HasValidSurface) {
                return;
            }

            if (e.PropertyName == BarcodeScanner.IsEnabledProperty.PropertyName)  {
                this.Debug("Enabled [{0}]", Element.IsEnabled);

                if (Element.IsEnabled && HasValidSurface) {
                    await Task.Run(CameraService.OpenCamera);
                } 

                if (!Element.IsEnabled) {
                    ReleaseCamera();
                }
            }

            if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) {
                this.Debug("ScannerActive [{0}]", Element.PreviewActive);

                if (Element.PreviewActive) {
                    await Task.Run(() => CameraService.StartPreview(Control.Holder));
                } 

                if (!Element.PreviewActive) {
                    CameraService.HaltPreview();
                }
            }

            if (e.PropertyName == BarcodeScanner.TorchProperty.PropertyName) {

                if (!Platform.HasFlashPermission) {
                    this.Debug("Unable to use flashlight: Android Manifest '{0}' permission not granted.", Android.Manifest.Permission.Flashlight);
                    return;
                }

                if (!Platform.HasFlash) {
                    this.Debug("Unable to use flashlight: Device's camera does not support flash.");
                    return;
                }

                this.Debug("Torch [{0}]", Element.Torch);

                CameraService.ToggleTorch(Element.Torch);
            }

            if (e.PropertyName == BarcodeScanner.BarcodeDecoderProperty.PropertyName) {
                this.Debug("Decoder state [{0}]", Element.BarcodeDecoder);

                if (Element.BarcodeDecoder) {
                    CameraService.StartDecoder();
                } 

                if (!Element.BarcodeDecoder) {
                    CameraService.StopDecoder();
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            this.Debug("Disposing");

            ReleaseCamera();

            base.Dispose(disposing);
        }

        private void ReleaseCamera()
        {
            CameraService?.ReleaseCamera();
            cameraServiceHolder = null;
        }
    }
}
