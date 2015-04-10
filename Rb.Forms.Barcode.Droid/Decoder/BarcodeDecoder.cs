using System;
using System.Threading;
using System.Threading.Tasks;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;
using ZXing;
using ZXing.Mobile;

namespace Rb.Forms.Barcode.Droid.Decoder
{
    public class BarcodeDecoder : ILog
    {
        private CancellationTokenSource tokenSource;
        private Task<String> decodeTask;
        private DateTime lastAnalysis = DateTime.UtcNow;

        private IBarcodeReader barcodeReader;
        private readonly RbConfig config;

        public BarcodeDecoder(RbConfig config)
        {
            this.config = config;

            var zxingNetOptions = new MobileBarcodeScanningOptions {
                AutoRotate = config.Rotate,
                TryHarder = config.TryHarder,
                TryInverted = config.TryInverted
            };

            barcodeReader = zxingNetOptions.BuildBarcodeReader();
            EnableDecoding();
        }

        public Task<String> DecodeAsync(byte[] bytes, int width, int height)
        {
            decodeTask = Task.Run(() => {
                return !CanDecode() ? null : decode(bytes, width, height);
            }, tokenSource.Token);

            return decodeTask;
        }

        public void EnableDecoding()
        {
            tokenSource = new CancellationTokenSource();
        }

        public void DisableDecoding()
        {
            tokenSource.Cancel();
        }

        private bool CanDecode()
        {
            if (!isTaskCompleted()) {
                return false;
            }

            if (!isEnoughTimeElapsedForNextAnalyzing()) {
                return false;
            }

            lastAnalysis = DateTime.UtcNow;

            return true;
        }

        private String decode(byte[] bytes, int width, int height)
        {
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
        }

        private bool isTaskCompleted()
        {
            return decodeTask == null || decodeTask.IsCompleted;
        }

        private bool isEnoughTimeElapsedForNextAnalyzing()
        {
            return (DateTime.UtcNow - lastAnalysis).TotalMilliseconds > config.DecoderDelay;
        }
    }
}

