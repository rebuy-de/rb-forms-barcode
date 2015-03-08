using Rb.Forms.Barcode.Pcl.Decoder;
using System.Threading.Tasks;
using System;

namespace Rb.Forms.Barcode.Pcl.Decoder
{
    public class DummyDecoder : AbstractDecoder
    {
        private readonly Random rnd = new Random();

        public override Task<string> DecodeAsync(byte[] bytes, int width, int height)
        {
            Func<String> decodeCallback = () => {
                var number = rnd.Next(100);

                if (number < 20) {
                    return String.Format("Random generated {0}", number);
                }

                return "";
            };

            return RunDecodeAsync(decodeCallback);
        }
    }
}

