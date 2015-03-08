using System;

namespace Rb.Forms.Barcode.Pcl.Decoder
{
    public interface IZxingNetMobileDecoder
    {
        Func<byte[], int, int, String> CreateDecoderCallback();
    }
}

