using System;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Pcl.Decoder;
using Rb.Forms.Barcode.Droid.Logger;
using ZXing;
using ZXing.Mobile;
using Android.Runtime;
using Rb.Forms.Barcode.Droid.Decoder;

[assembly: Xamarin.Forms.Dependency (typeof (ZxingNetMobileFactory))]
namespace Rb.Forms.Barcode.Droid.Decoder
{
    [Preserve(AllMembers=true)] 
    public class ZxingNetMobileFactory : IZxingNetMobileFactory, ILog
    {
        private IBarcodeReader barcodeReader;
        private readonly MobileBarcodeScanningOptions options = new MobileBarcodeScanningOptions();

        public ZxingNetMobileFactory()
        {
            barcodeReader = options.BuildBarcodeReader();
        }

        public Func<byte[], int, int, string> CreateDecoderCallback()
        {
            return (bytes, width, height) => {
                try {   
                    var source = new PlanarYUVLuminanceSource(bytes, width, height, 0, 0, width, height, false);
                    var rotated = source.rotateCounterClockwise();
                    var result = barcodeReader.Decode(rotated);

                    if (null == result) {
                        return "";
                    }

                    return result.Text;
                } catch (Exception ex) {
                    this.Debug(ex.ToString());
                    return "";
                }
            };
        }
    }
}

