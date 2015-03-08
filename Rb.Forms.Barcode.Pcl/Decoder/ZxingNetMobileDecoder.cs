using System;
using System.Threading.Tasks;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Pcl.Decoder;
using Xamarin.Forms;

namespace Rb.Forms.Barcode.Pcl.Decoder
{
    public class ZxingNetMobileDecoder : AbstractDecoder, ILog
    {
        private Func<byte[], int, int, String> zxingCallback;

        public ZxingNetMobileDecoder()
        {
            var callbackFactory = DependencyService.Get<IZxingNetMobileFactory>();
            zxingCallback = callbackFactory.CreateDecoderCallback();
        }

        public override Task<String> DecodeAsync(byte[] bytes, int width, int height)
        {
            Func<String> wrappedCallback = () => zxingCallback(bytes, width, height);

            return RunDecodeAsync(wrappedCallback);
        }
    }
}

