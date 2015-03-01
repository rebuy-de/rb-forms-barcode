using System;
using System.Threading;
using System.Threading.Tasks;
using Rb.Forms.Barcode.Droid.Logger;
using ZXing;
using ZXing.Mobile;

namespace Rb.Forms.Barcode.Droid.Decoder
{
    public class BarcodeDecoder : ILog
    {
        private CancellationTokenSource cancellationTokenSource;
        private Task<String> currentTask;
        private DateTime lastPreviewScanAnalysis = DateTime.UtcNow;

        private IBarcodeReader barcodeReader;
        private readonly MobileBarcodeScanningOptions options = new MobileBarcodeScanningOptions();

        public BarcodeDecoder()
        {
            options.DelayBetweenAnalyzingFrames = 400;
            barcodeReader = options.BuildBarcodeReader();
            RefreshToken();
        }

        public Task<String> DecodeAsync(byte[] bytes, int width, int height)
        {
            if (isTaskIncomplete()) {
                return null;
            }

            if (!isEnoughTimeElapsedForNextAnalyzing()) {
                return null;
            }

            lastPreviewScanAnalysis = DateTime.UtcNow;

            currentTask = Task.Run(() => {
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
            }, cancellationTokenSource.Token);

            return currentTask;
        }

        public void CancelDecoding()
        {
            cancellationTokenSource.Cancel();
        }

        public void RefreshToken()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        private bool isTaskIncomplete()
        {
            return currentTask != null && !currentTask.IsCompleted;
        }

        private bool isEnoughTimeElapsedForNextAnalyzing()
        {
            return (DateTime.UtcNow - lastPreviewScanAnalysis).TotalMilliseconds > options.DelayBetweenAnalyzingFrames;
        }
    }
}

