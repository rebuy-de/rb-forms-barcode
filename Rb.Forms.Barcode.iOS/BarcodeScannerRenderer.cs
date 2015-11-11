using System;
using Xamarin.Forms;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.iOS;
using Xamarin.Forms.Platform.iOS;
using AVFoundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using System.ComponentModel;
using CoreFoundation;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;
using Rb.Forms.Barcode.Pcl.Extensions;
using Rb.Forms.Barcode.iOS.Extensions;

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.iOS
{
	public class BarcodeScannerRenderer : ViewRenderer, IAVCaptureMetadataOutputObjectsDelegate
    {
		private NSObject orientationObserverToken;

		private static Configuration configuraiton;
		private AVCaptureSession session;
		private AVCaptureDevice device;
		private AVCaptureDeviceInput input;
		private AVCaptureMetadataOutput output;
		private AVCaptureVideoPreviewLayer captureVideoPreviewLayer;
		private UIView view;
		private BarcodeScanner barcodeScanner;

		public static void Init(Configuration configuraiton) 
		{
			BarcodeScannerRenderer.configuraiton = configuraiton;
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
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			barcodeScanner = (BarcodeScanner) Element;
			initScanner();

			view = new UIView(CGRect.Empty);
			view.BackgroundColor = UIColor.Gray;
			view.Layer.AddSublayer(captureVideoPreviewLayer);

			session.StartRunning();

			orientationObserverToken = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, updateViewOnOrientationChanged);
			barcodeScanner.OnCameraOpened();
			barcodeScanner.OnPreviewActivated();

			SetNativeControl(view);	
		}


		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			updateSize(e);
			updateCameraRunningState(e);
			updateTorch(e);
		}

		private void updateViewOnOrientationChanged(NSNotification notification) 
		{
			var previewLayerConnection = captureVideoPreviewLayer.Connection;
			captureVideoPreviewLayer.Frame = new CGRect(0, 0, Element.Width, Element.Height);

			if (previewLayerConnection.SupportsVideoOrientation) {
				previewLayerConnection.VideoOrientation = (AVCaptureVideoOrientation) UIApplication.SharedApplication.StatusBarOrientation;
			}
		}

		private void updateCameraRunningState(PropertyChangedEventArgs e)
		{
			if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) {
				if (barcodeScanner.PreviewActive) {
					session.StartRunning();
					orientationObserverToken = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, updateViewOnOrientationChanged);
					barcodeScanner.OnPreviewActivated();
				} else {
					session.StopRunning();
					NSNotificationCenter.DefaultCenter.RemoveObserver(orientationObserverToken);
					barcodeScanner.OnPreviewDeactivated();
				}
			}

			if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName) {
				if (barcodeScanner.IsEnabled) {
					session.StartRunning();
					orientationObserverToken = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, updateViewOnOrientationChanged);
					barcodeScanner.OnCameraOpened();
				} else {
					session.StopRunning();
					NSNotificationCenter.DefaultCenter.RemoveObserver(orientationObserverToken);
					barcodeScanner.OnCameraReleased();
				}
			}
		}
			
		private void updateSize(PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.WidthProperty.PropertyName || e.PropertyName == VisualElement.HeightProperty.PropertyName) {
				captureVideoPreviewLayer.Frame = new CGRect(0, 0, Element.Width, Element.Height);
			}
		}

		private void updateTorch(PropertyChangedEventArgs e) 
		{
			if (e.PropertyName == BarcodeScanner.TorchProperty.PropertyName && device.TorchAvailable) {
				NSError error;
				device.LockForConfiguration(out error);

				if (barcodeScanner.Torch) {				
					device.TorchMode = AVCaptureTorchMode.On;
				} else {
					device.TorchMode = AVCaptureTorchMode.Off;
				}

				device.UnlockForConfiguration();
			}
		}

		private void initScanner() 
		{
			device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			input = AVCaptureDeviceInput.FromDevice(this.device);
			session = new AVCaptureSession();
			session.AddInput(input);

			output = new AVCaptureMetadataOutput();
			output.SetDelegate(this, DispatchQueue.MainQueue);

			session.AddOutput(output);
			output.MetadataObjectTypes = configuraiton.Barcodes.ConvertToIOS();

			captureVideoPreviewLayer = AVCaptureVideoPreviewLayer.FromSession(session);
			captureVideoPreviewLayer.Frame = CGRect.Empty;
			captureVideoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
		}
    }
}