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

[assembly: ExportRenderer(typeof(BarcodeScanner), typeof(BarcodeScannerRenderer))]
namespace Rb.Forms.Barcode.iOS
{
	public class BarcodeScannerRenderer : ViewRenderer, IAVCaptureMetadataOutputObjectsDelegate
    {
		private AVCaptureSession session;
		private AVCaptureDevice device;
		private AVCaptureDeviceInput input;
		private AVCaptureMetadataOutput output;
		private AVCaptureVideoPreviewLayer captureVideoPreviewLayer;
		private NSNotificationCenter defaultCenter = new NSNotificationCenter();
		private UIView view;
		private static AVMetadataObjectType barcodeTypes;
		private BarcodeScanner barcodeScanner;

		private readonly Dictionary<AVMetadataObjectType, RebuyBarcode.BarcodeFormat> formatMapping =
			new Dictionary<AVMetadataObjectType, RebuyBarcode.BarcodeFormat> {
				{ AVMetadataObjectType.Code128Code, RebuyBarcode.BarcodeFormat.Code128 },
				{ AVMetadataObjectType.Code39Code, RebuyBarcode.BarcodeFormat.Code39 },
				{ AVMetadataObjectType.Code93Code, RebuyBarcode.BarcodeFormat.Code93 },
				{ AVMetadataObjectType.DataMatrixCode, RebuyBarcode.BarcodeFormat.DataMatrix },
				{ AVMetadataObjectType.EAN13Code, RebuyBarcode.BarcodeFormat.Ean13 },
				{ AVMetadataObjectType.EAN8Code, RebuyBarcode.BarcodeFormat.Ean8 },
				{ AVMetadataObjectType.ITF14Code, RebuyBarcode.BarcodeFormat.Itf },
				{ AVMetadataObjectType.PDF417Code, RebuyBarcode.BarcodeFormat.Pdf417 },
				{ AVMetadataObjectType.QRCode, RebuyBarcode.BarcodeFormat.QrCode },
				{ AVMetadataObjectType.UPCECode, RebuyBarcode.BarcodeFormat.UpcE },
			};

		public static void Init(Configuration configuraiton) 
		{
			barcodeTypes = AVMetadataObjectType.EAN8Code | AVMetadataObjectType.EAN13Code;
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
			NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, updateViewOnOrientationChanged);
			barcodeScanner.PreviewActivatedCommand.Raise();
			SetNativeControl(view);	
		}


		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			updateSize(e);
			updatePreviewActive(e);
			updateTorch(e);
		}

		[Export("captureOutput:didOutputMetadataObjects:fromConnection:")]
		public void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
		{
			foreach (var metadata in metadataObjects) {
				var readableMetaData = (AVMetadataMachineReadableCodeObject) metadata;
				if (formatMapping.ContainsKey(metadata.Type)) {
					barcodeScanner.Barcode = new Barcode.Pcl.Barcode(readableMetaData.StringValue, formatMapping[metadata.Type]);
				}
			}
		}

		private void updateViewOnOrientationChanged(NSNotification notification) 
		{
			captureVideoPreviewLayer.Frame = new CGRect(0, 0, Element.Width, Element.Height);

			var previewLayerConnection = captureVideoPreviewLayer.Connection;

			if (previewLayerConnection.SupportsVideoOrientation) {
				previewLayerConnection.VideoOrientation = (AVCaptureVideoOrientation) UIApplication.SharedApplication.StatusBarOrientation;
			}
		}

		private void updatePreviewActive(PropertyChangedEventArgs e)
		{
			if (e.PropertyName == BarcodeScanner.PreviewActiveProperty.PropertyName) {
				if (barcodeScanner.PreviewActive) {
					session.StartRunning();
				} else {
					session.StopRunning();
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
			output.MetadataObjectTypes = barcodeTypes;

			captureVideoPreviewLayer = AVCaptureVideoPreviewLayer.FromSession(session);
			captureVideoPreviewLayer.Frame = CGRect.Empty;
			captureVideoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
		}
    }
}