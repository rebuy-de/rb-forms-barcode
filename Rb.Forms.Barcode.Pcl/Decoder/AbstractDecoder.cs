using System;
using System.Threading;
using System.Threading.Tasks;
using Rb.Forms.Barcode.Pcl.Decoder;

namespace Rb.Forms.Barcode.Pcl.Decoder
{
    public abstract class AbstractDecoder : IDecoder
    {
        private const int delay = 400;
        private DateTime lastAnalysis = DateTime.UtcNow;
        private CancellationTokenSource tokenSource;

        private Task<String> decodeTask;

        public abstract Task<String> DecodeAsync(byte[] bytes, int width, int height);

        protected virtual Task<String> RunDecodeAsync(Func<String> decodeCallback)
        {
            decodeTask = Task.Run(() => {
                return !CanDecode() ? null : decodeCallback();
            }, tokenSource.Token);

            return decodeTask;
        }

        public virtual void StartDecoding()
        {
            tokenSource = new CancellationTokenSource();
        }

        public virtual void PauseDecoding()
        {
            tokenSource.Cancel();
        }

        protected bool CanDecode()
        {
            if (isTaskIncomplete()) {
                return false;
            }

            if (!isEnoughTimeElapsedForNextAnalyzing()) {
                return false;
            }

            lastAnalysis = DateTime.UtcNow;

            return true;
        }

        private bool isTaskIncomplete()
        {
            return decodeTask != null && !decodeTask.IsCompleted;
        }

        private bool isEnoughTimeElapsedForNextAnalyzing()
        {
            return (DateTime.UtcNow - lastAnalysis).TotalMilliseconds > delay;
        }
    }
}

