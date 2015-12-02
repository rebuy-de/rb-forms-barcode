using Xamarin.Forms;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.iOS;
using Xamarin.Forms.Platform.iOS;
using AVFoundation;
using UIKit;
using CoreGraphics;
using Foundation;
using System.ComponentModel;
using CoreFoundation;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;
using Rb.Forms.Barcode.iOS.Extensions;
using System.Diagnostics;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.iOS.Logger;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.iOS
{
    public class BarcodeScannerRenderer : ViewRenderer, IAVCaptureMetadataOutputObjectsDelegate, ILog
    {
        private NSObject orientationObserverToken;

        private static Configuration configuration;
        private AVCaptureSession session;
        private AVCaptureDevice device;
        private AVCaptureDeviceInput input;
        private AVCaptureMetadataOutput output;
        private AVCaptureVideoPreviewLayer captureVideoPreviewLayer;
        private UIView view;
        private BarcodeScanner barcodeScanner;

        public static void Init(Configuration configuration)
        {
            BarcodeScannerRenderer.configuration = configuration;
        }

        [Export("captureOutput:didOutputMetadataObjects:fromConnection:")]
        public void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            if (!barcodeScanner.BarcodeDecoder) {
                return;
            }

            foreach (var metadata in metadataObjects) {
                barcodeScanner.Barcode = new RebuyBarcode(
                    ((AVMetadataMachineReadableCodeObject) metadata).StringValue, 
                    metadata.Type.ConvertToPcl()
                );
                return;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            barcodeScanner = (BarcodeScanner) Element;

            if (barcodeScanner == null) {
                return;
            }

            if (!initScanner()) {
                return;
            }

            view = new UIView(CGRect.Empty);
            view.BackgroundColor = UIColor.Gray;
            view.Layer.AddSublayer(captureVideoPreviewLayer);

            startSession();

            barcodeScanner.OnCameraOpened();
            barcodeScanner.OnPreviewActivated();

            SetNativeControl(view); 
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (input == null) {
                return;
            }
            updateSize(e);
            updateCameraRunningState(e);
            updateTorch(e);
        }

        private void updateViewOnOrientationChanged(NSNotification notification)
        {
            if (Element == null) {
                removeOrientationObserver();

                return;
            }
            var previewLayerConnection = captureVideoPreviewLayer.Connection;
            captureVideoPreviewLayer.Frame = new CGRect(0, 0, Element.Width, Element.Height);

            if (previewLayerConnection.SupportsVideoOrientation) {
                previewLayerConnection.VideoOrientation = getDeviceOrientation();
                this.Debug("AVCaptureVideoOrientation changed {0}", previewLayerConnection.VideoOrientation);
            }
        }
            
        private void updateCameraRunningState(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) {
                if (barcodeScanner.PreviewActive) {
                    startSession();
                    barcodeScanner.OnPreviewActivated();
                } else {
                    stopSession();
                    barcodeScanner.OnPreviewDeactivated();
                }
            }

            if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName) {
                if (barcodeScanner.IsEnabled) {
                    startSession();
                    barcodeScanner.OnCameraOpened();
                } else {
                    stopSession();
                    barcodeScanner.OnCameraReleased();
                }
            }
        }

        private void startSession() 
        {
            session.StartRunning();
            captureVideoPreviewLayer.Connection.VideoOrientation = getDeviceOrientation();
            this.Debug("StartRunning");
            addOrientationObserver();
        }

        private void stopSession() 
        {
            session.StopRunning();
            this.Debug("StopRunning");
            removeOrientationObserver();
        }

        private void addOrientationObserver()
        {
            orientationObserverToken = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, updateViewOnOrientationChanged);
        }

        private void removeOrientationObserver()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(orientationObserverToken);
        }

        private void updateSize(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.WidthProperty.PropertyName || e.PropertyName == VisualElement.HeightProperty.PropertyName) {
                captureVideoPreviewLayer.Frame = new CGRect(0, 0, Element.Width, Element.Height);
                this.Debug("Preview Frame changed to {0}", captureVideoPreviewLayer.Frame);
            }
        }

        private void updateTorch(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BarcodeScanner.TorchProperty.PropertyName && device.TorchAvailable) {
                NSError error;
                device.LockForConfiguration(out error);

                if (error != null) {
                    this.Debug("Torch error");

                    return;
                }

                device.TorchMode = barcodeScanner.Torch ? AVCaptureTorchMode.On : AVCaptureTorchMode.Off;

                device.UnlockForConfiguration();
                this.Debug("Torch AVCaptureTorchMode {0}", barcodeScanner.Torch);
            }
        }

        private bool initScanner()
        {
            device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            if (device == null) {
                this.Debug("AVCaptureDevice is null");

                return false;
            }

            input = AVCaptureDeviceInput.FromDevice(device);
          
            if (input == null) {
                this.Debug("AVCaptureDeviceInput is null");

                return false;
            }

            output = new AVCaptureMetadataOutput();
            output.SetDelegate(this, DispatchQueue.MainQueue);

            session = new AVCaptureSession();
            session.AddInput(input);
            session.AddOutput(output);
            output.MetadataObjectTypes = configuration.Barcodes.ConvertToIOS();

            captureVideoPreviewLayer = AVCaptureVideoPreviewLayer.FromSession(session);
            captureVideoPreviewLayer.Frame = CGRect.Empty;
            captureVideoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            captureVideoPreviewLayer.Connection.VideoOrientation = getDeviceOrientation();

            return true;
        }

        private AVCaptureVideoOrientation getDeviceOrientation()
        {
            return (AVCaptureVideoOrientation) UIApplication.SharedApplication.StatusBarOrientation;
        }
    }
}
