using System;

using Xamarin.Forms;
using System.Diagnostics;
using Rb.Forms.Barcode.Pcl.Extensions;
using System.Windows.Input;
using System.ComponentModel;

namespace Rb.Forms.Barcode.Pcl
{
    public class BarcodeScanner : View
    {     
        /// <summary>
        /// Command property gets called only when the barcode text changes.
        /// </summary>
        public static BindableProperty BarcodeChangedCommandProperty = BindableProperty.Create(
            propertyName: "BarcodeChangedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(BarcodeScanner),
            defaultValue: null
        );

        /// <summary>
        /// Command property gets called every time when a barcode is decoded from the preview, even if the value is the same as the previews one.
        /// </summary>
        public static BindableProperty BarcodeDecodedCommandProperty = BindableProperty.Create(
            propertyName: "BarcodeDecodedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(BarcodeScanner),
            defaultValue: null
        );

        /// <summary>
        /// Command property gets called as soon as the surfaces starts previewing.
        /// </summary>
        public static BindableProperty PreviewActivatedCommandProperty = BindableProperty.Create(
            propertyName: "PreviewActivatedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(BarcodeScanner),
            defaultValue: null
        );

        /// <summary>
        /// Command property gets called when the surfaces stops previewing.
        /// </summary>
        public static BindableProperty PreviewDeactivatedCommandProperty = BindableProperty.Create(
            propertyName: "PreviewDeactivatedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(BarcodeScanner),
            defaultValue: null
        );

        /// <summary>
        /// Command property gets called after the camera was opened.
        /// </summary>
        public static BindableProperty CameraOpenedCommandProperty = BindableProperty.Create(
            propertyName: "CameraOpenedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(BarcodeScanner),
            defaultValue: null
        );

        /// <summary>
        /// Command property gets called after the camera was released.
        /// </summary>
        public static BindableProperty CameraReleasedCommandProperty = BindableProperty.Create(
            propertyName: "CameraReleasedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(BarcodeScanner),
            defaultValue: null
        );

        /// <summary>
        /// OneWay source to target binding for the current barcode.
        /// </summary>
        public static readonly BindableProperty BarcodeProperty =
            BindableProperty.Create<BarcodeScanner, Barcode>(
                getter: view => view.Barcode,
                defaultValue: default(Barcode),
                propertyChanged: OnBarcodeChanged
            );

        /// <summary>
        /// Gets or sets the current barcode.
        /// </summary>
        /// <value>The barcode.</value>
        public Barcode Barcode
        {
            get { return (Barcode) GetValue(BarcodeProperty); }
            set {
                SetValue(BarcodeProperty, value);
                OnBarcodeDecoded(value);
            }
        }

        /// <summary>
        /// TwoWay binding to control the scanner preview and decoder state.
        /// If set to true the preview is active, false deactivates the preview image.
        /// </summary>
        public static readonly BindableProperty PreviewActiveProperty =
            BindableProperty.Create<BarcodeScanner, bool>(
                getter: view => view.PreviewActive,
                defaultValue: true,
                defaultBindingMode: BindingMode.TwoWay
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
                getter: view => view.BarcodeDecoder,
                defaultValue: true,
                defaultBindingMode: BindingMode.TwoWay
            );

        /// <summary>
        /// Command gets called only when the barcode text changes.
        /// </summary>
        public ICommand BarcodeChangedCommand {
            get { return (ICommand) GetValue(BarcodeChangedCommandProperty); }
            set { SetValue(BarcodeChangedCommandProperty, value); }
        }

        /// <summary>
        /// Command gets called every time when a barcode is decoded from the preview, even if the value is the same as the previews one.
        /// </summary>
        public ICommand BarcodeDecodedCommand {
            get { return (ICommand) GetValue(BarcodeDecodedCommandProperty); }
            set { SetValue(BarcodeDecodedCommandProperty, value); }
        }

        /// <summary>
        /// Command gets called as soon as the surfaces starts previewing.
        /// </summary>
        public ICommand PreviewActivatedCommand {
            get { return (ICommand) GetValue(PreviewActivatedCommandProperty); }
            set { SetValue(PreviewActivatedCommandProperty, value); }
        }

        /// <summary>
        /// Command gets called when the surfaces stops previewing.
        /// </summary>
        public ICommand PreviewDeactivatedCommand {
            get { return (ICommand) GetValue(PreviewDeactivatedCommandProperty); }
            set { SetValue(PreviewDeactivatedCommandProperty, value); }
        }

        /// <summary>
        /// Command gets called after the camera was opened.
        /// </summary>
        public ICommand CameraOpenedCommand {
            get { return (ICommand) GetValue(CameraOpenedCommandProperty); }
            set { SetValue(CameraOpenedCommandProperty, value); }
        }

        /// <summary>
        /// Command gets called after the camera was released.
        /// </summary>
        public ICommand CameraReleasedCommand {
            get { return (ICommand) GetValue(CameraReleasedCommandProperty); }
            set { SetValue(CameraReleasedCommandProperty, value); }
        }

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
        public event EventHandler<BarcodeEventArgs> BarcodeChanged;

        /// <summary>
        /// Occurs every time when a barcode is decoded from the preview, even if the value is the same as the previews one.
        /// </summary>
        public event EventHandler<BarcodeEventArgs> BarcodeDecoded;

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
            CameraOpenedCommand.Raise();
            CameraOpened.Raise(this, EventArgs.Empty);
        }

        public void OnCameraReleased()
        {
            CameraReleasedCommand.Raise();
            CameraReleased.Raise(this, EventArgs.Empty);
        }

        private static void OnBarcodeChanged(BindableObject bindable, Barcode oldValue, Barcode newBarcode)
        {
            Debug.WriteLine("[ScannerView] OnBarcodeChanged [{0} - {1}]", newBarcode.Result, newBarcode.Format);

            var b = (BarcodeScanner) bindable;
            b.BarcodeChangedCommand.Raise(newBarcode);
            b.BarcodeChanged.Raise(b, new BarcodeEventArgs(newBarcode));
        }

        private void OnBarcodeDecoded(Barcode barcode)
        {
            Debug.WriteLine("[ScannerView] OnBarcodeDecoded [{0} - {1}]", barcode.Result, barcode.Format);

            BarcodeDecodedCommand.Raise(barcode);
            BarcodeDecoded.Raise(this, new BarcodeEventArgs(barcode));
        }

        public void OnPreviewActivated()
        {
            PreviewActivatedCommand.Raise();
            PreviewActivated.Raise(this, EventArgs.Empty);
        }

        public void OnPreviewDeactivated()
        {
            PreviewDeactivatedCommand.Raise();
            PreviewDeactivated.Raise(this, EventArgs.Empty);
        }
    }   
}
