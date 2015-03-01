using System;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Droid.Logger;
using Rb.Forms.Barcode.Droid.Decoder;

using AndroidCamera = Android.Hardware.Camera;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class PreviewFrameCallback : Java.Lang.Object, AndroidCamera.IPreviewCallback, ILog
    {
        private readonly BarcodeDecoder barcodeDecoder;
        private readonly BarcodeScanner scanner;

        public PreviewFrameCallback(BarcodeDecoder decoder, BarcodeScanner scanner)
        {
            barcodeDecoder = decoder;
            this.scanner = scanner;
        }

        async public void OnPreviewFrame(byte[] bytes, AndroidCamera camera)
        {
            var previewSize = camera.GetParameters().PreviewSize;
            var decoder = barcodeDecoder.DecodeAsync(bytes, previewSize.Width, previewSize.Height);

            if (null == decoder) {
                return;
            }

            var barcode = await decoder;

            if (!string.IsNullOrWhiteSpace(barcode)) {
                scanner.RaiseBarcodeFoundEvent(barcode);
            }
        }
    }
}
#pragma warning restore 618
