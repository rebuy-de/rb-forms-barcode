using System;
using System.Threading.Tasks;

namespace Rb.Forms.Barcode.Pcl.Decoder
{
    public interface IDecoder
    {
        Task<String> DecodeAsync(byte[] bytes, int width, int height);
        void StartDecoding();
        void PauseDecoding();
    }
}

