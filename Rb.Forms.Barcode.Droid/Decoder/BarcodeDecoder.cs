using System;
using System.Threading;
using System.Threading.Tasks;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;
using ZXing;
using ZXing.Mobile;
using Android.Graphics;
using System.Diagnostics;

namespace Rb.Forms.Barcode.Droid.Decoder
{
    public class BarcodeDecoder : ILog
    {
        private CancellationTokenSource tokenSource;
        private Task<String> decodeTask;
        private DateTime lastAnalysis = DateTime.UtcNow;

        private IBarcodeReader barcodeReader;
        private readonly RbConfig config;
        private Stopwatch stopwatch;


        public BarcodeDecoder(RbConfig config)
        {
            this.config = config;

            var zxingNetOptions = new MobileBarcodeScanningOptions {
                AutoRotate = config.Rotate,
                TryHarder = config.TryHarder,
                TryInverted = config.TryInverted,
            };

            foreach (var format in config.Barcodes) {
                zxingNetOptions.PossibleFormats.Add(format);
            }

            barcodeReader = zxingNetOptions.BuildBarcodeReader();

            if (config.Metrics) {
                stopwatch = new Stopwatch();
            }

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

                if (config.Metrics) {
                    stopwatch.Restart();
                }

                LuminanceSource source = new PlanarYUVLuminanceSource(bytes, width, height, 0, 0, width, height, false);

                if (!config.Rotate) {
                    source = source.rotateCounterClockwise();
                }

                var result = barcodeReader.Decode(source);

                if (config.Metrics) {
                    stopwatch.Stop();
                    this.Debug("[Metric] Time to decode [{0} MS]", stopwatch.ElapsedMilliseconds);
                }

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

