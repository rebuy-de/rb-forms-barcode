using System;

using Xamarin.Forms;
using System.Diagnostics;
using Rb.Forms.Barcode.Pcl.Extensions;

namespace Rb.Forms.Barcode.Pcl
{
    public class BarcodeScanner : View
   {
        /// <summary>
        /// OneWay source to target binding for the current barcode.
        /// </summary>
        public static readonly BindableProperty BarcodeProperty =
            BindableProperty.Create<BarcodeScanner, String>(
                p => p.Barcode, default(string), propertyChanged: OnBarcodeChanged
            );

        /// <summary>
        /// Gets or sets the current barcode.
        /// </summary>
        /// <value>The barcode.</value>
        public String Barcode
        {
            get { return (String) GetValue(BarcodeProperty); }
            set { SetValue(BarcodeProperty, value); OnBarcodeRead(value); }
        }

        /// <summary>
        /// TwoWay binding to control the scanner preview and decoder state.
        /// If set to true the preview is active, false deactivates the preview image.
        /// </summary>
        public static readonly BindableProperty PreviewActiveProperty =
            BindableProperty.Create<BarcodeScanner, bool>(
                p => p.PreviewActive, default(bool), BindingMode.TwoWay, propertyChanged: OnPreviewActiveChanged
            );

        /// <summary>
        /// Gets or controlls the current preview state.
        /// </summary>
        /// <value>The barcode.</value>
        public bool PreviewActive
        {
            get { return (bool) GetValue(PreviewActiveProperty); }
            set { SetValue(PreviewActiveProperty, value); }
        }

        /// <summary>
        /// TwoWay binding for barcode decoder control.
        /// If set to true the decoder tries to read the barcode from the surface image, false deactivates the decoder.
        /// </summary>
        public static readonly BindableProperty BarcodeDecoderProperty =
            BindableProperty.Create<BarcodeScanner, bool>(
                p => p.BarcodeDecoder, default(bool), BindingMode.TwoWay
            );

        /// <summary>
        /// Gets or controlls the decoder state.
        /// </summary>
        /// <value>The barcode.</value>
        public bool BarcodeDecoder
        {
            get { return (bool) GetValue(BarcodeDecoderProperty); }
            set { SetValue(BarcodeDecoderProperty, value); }
        }

        /// <summary>
        /// Occurs only when the barcode text changes.
        /// </summary>
        public event EventHandler<BarcodeFoundEventArgs> BarcodeChanged;

        /// <summary>
        /// Occurs every time when a barcode is decoded from the preview, even if the value is the same as the previews one.
        /// </summary>
        public event EventHandler<BarcodeReadEventArgs> BarcodeDecoded;

        /// <summary>
        /// Occurs as soon as the surfaces starts previewing.
        /// </summary>
        public event EventHandler<EventArgs> PreviewActivated;

        /// <summary>
        /// Occurs when the surfaces stops previewing.
        /// </summary>
        public event EventHandler<EventArgs> PreviewDeactivated;

        /// <summary>
        /// Occurs after the camera was opened.
        /// </summary>
        public event EventHandler<EventArgs> CameraOpened;

        /// <summary>
        /// Occurs after the camera was released.
        /// </summary>
        public event EventHandler<EventArgs> CameraReleased;

        public void OnCameraOpened()
        {
            CameraOpened.Raise(this, EventArgs.Empty);
        }

        public void OnCameraReleased()
        {
            CameraReleased.Raise(this, EventArgs.Empty);
        }

        private static void OnBarcodeChanged(BindableObject bindable, String oldValue, String newBarcode)
        {
            Debug.WriteLine("[ScannerView] OnBarcodeChanged [{0}]", newBarcode, null);
            var b = (BarcodeScanner) bindable;

            b.BarcodeChanged.Raise(b, new BarcodeFoundEventArgs(newBarcode));
        }

        private void OnBarcodeRead(String barcode)
        {
            Debug.WriteLine("[ScannerView] OnBarcodeRead [{0}]", barcode, null);

            BarcodeDecoded.Raise(this, new BarcodeReadEventArgs(barcode));
        }

        private static void OnPreviewActiveChanged(BindableObject bindable, bool oldState, bool newState)
        {
            var b = (BarcodeScanner) bindable;

            Debug.WriteLine("[ScannerView] OnPreviewActiveChanged to [{0}]", newState);

            if (newState) {
                b.PreviewActivated.Raise(b, new EventArgs());
            }

            if (!newState) {
                b.PreviewDeactivated.Raise(b, new EventArgs());
            }
        }
    }   
}
