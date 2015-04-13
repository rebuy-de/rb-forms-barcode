using System;
using System.Collections.Generic;
using AndroidCamera = Android.Hardware.Camera;
using ZXing;
using Xamarin.Forms;

namespace Rb.Forms.Barcode.Droid
{
    public class RbConfig
    {


        private Rectangle barcodeArea = new Rectangle(0, 0, 100, 100);

        public enum Quality {
            High,
            Medium,
            Low
        };


        /// <summary>
        /// Enable detection of picky android devices (e.g.: samsung).
        /// If enabled all features except focus mode configuration wont be activated when a picky device is detected.
        /// </summary>
        public bool PickyDeviceDetection = true;

        /// <summary>
        /// List of focus modes that should be checked if they are supported by the device.
        /// The list is in FIFO priority order. The first supported mode will be used.
        /// Most devices should at least support FocusModeAuto.
        /// </summary>
        public IList<String> FocusModes = new List<string> {
            AndroidCamera.Parameters.FocusModeContinuousPicture,
            AndroidCamera.Parameters.FocusModeContinuousVideo,
            AndroidCamera.Parameters.FocusModeAuto,
        };

        /// <summary>
        /// Defines the preview resolution quality. This setting has direct impact on the decoder performance.
        /// 
        /// * The higher the setting the better and clearer the preview image but the decoder performance gets
        ///   worse because of the amount of data (pixel) to process.
        /// * Medium is a trade off between preview quality and performance. (recommended setting)
        /// * Low preview resolution leads to the best decoder performance but the preview image quality suffers.
        ///   Please note that the quality may be so low that the barcode is not readable any more.
        /// 
        /// The default device dependant preview resolution will be used as fallback.
        /// </summary>
        /// <remarks>
        /// The aspect ratio of the device display is considered when selecting a preview resolution.
        /// <seealso cref="RbConfig.AspectRatioThreshold" />
        /// </remarks>
        public Quality PreviewResolution = Quality.Medium;

        /// <summary>
        /// Threshold value that controls if a suggested resolution should be discarded when its aspect ratio is
        /// different than the screen ratio.
        /// A bigger tolerance level means more resolutions to pick from but can result in distorted preview images.
        /// <seealso cref="RbConfig.PreviewResolution"/>
        /// </summary>
        public double AspectRatioThreshold = 0.15;

        /// <summary>
        /// List of barcode types the decoder should look out for.
        /// Its recommended to narrow the list down to increase decoder performance.
        /// If no format is specified all available formats will be registered.
        /// <seealso cref="ZXing.BarcodeFormat"/>
        /// </summary>
        public IList<BarcodeFormat> Barcodes = new List<BarcodeFormat>();

        /// <summary>
        /// Show performance related metrics in the application output.
        /// </summary>
        public bool Metrics = false;

        /// <summary>
        /// Rectangle area where the decoder should look for a barcode on the preview image. Increases the chance to
        /// find a barcode and has positive effect on decoder speed (less pixels to scan). If the image of the barcode
        /// is not within this area no barcode will be found.
        /// 
        /// Calculation of the rectangle boundaries is percentage based.
        /// Given values shall not exceed the range from 0 to 100.
        /// Calculation of the rectangle is based on portrait mode orientation.
        /// The starting point is the lower left corner, x and y being 0.
        /// </summary>
        /// <example> 
        /// Image: width = 1280, height = 720
        /// Rectangle: x = 33, y = 0, width = 33, height = 100
        /// Result: x = 422px, y = 0, 422px, 720px
        /// </example>
        public Rectangle BarcodeArea {
            get {
                return barcodeArea;
            }
            set {
                if (isOutOfBoundaries(value.X)) {
                    throw new Rb.Forms.Barcode.Pcl.OutOfBoundsException("X is out of bounds.");
                }

                if (isOutOfBoundaries(value.Y)) {
                    throw new Rb.Forms.Barcode.Pcl.OutOfBoundsException("Y is out of bounds.");
                }

                if (isOutOfBoundaries(value.Width)) {
                    throw new Rb.Forms.Barcode.Pcl.OutOfBoundsException("Width is out of bounds.");
                }

                if (isOutOfBoundaries(value.Height)) {
                    throw new Rb.Forms.Barcode.Pcl.OutOfBoundsException("Height is out of bounds.");
                }
            }
        }

        /// <summary>
        /// Enable or disable metering area configuration.
        /// If false no metering areas will be set even if the device claims to support it.
        /// </summary>
        public bool MeteringAreas = false;

        /// <summary>
        /// Enable or disable focus area configuration.
        /// If false no focus areas will be set even if the device claims to support it.
        /// </summary>
        public bool FocusAreas = false;

        /// <summary>
        /// Enable or disable scene mode configuration.
        /// If false no scene mode will be set even if the device claims to support it.
        /// </summary>
        public bool SceneMode = false;

        /// <summary>
        /// Enable or disable video stabilization configuration.
        /// If false video stabilization will not be enabled even if the device claims to support it.
        /// </summary>
        public bool VideoStabilization = false;

        /// <summary>
        /// Enable or disable auto whitebalance configuration.
        /// If false auto whitebalance will not be enabled even if the device claims to support it.
        /// </summary>
        public bool WhiteBalance = false;

        /// <summary>
        /// Delay between decoder runs in miliseconds.
        /// A lower delay results in higher performance degration (more decoder runs)
        /// </summary>
        public int DecoderDelay = 150;

        /// <summary>
        /// Rotate image until a barcode was found. This will happen at max four times.
        /// </summary>
        public bool Rotate = false;

        /// <summary>
        /// Spend more time to try to find a barcode. Optimize for accuracy, not speed.
        /// </summary>
        public bool TryHarder = false;

        /// <summary>
        /// Inverted image colors when the first pass yields no barcode.
        /// </summary>
        public bool TryInverted = false;

        private bool isOutOfBoundaries(double value)
        {
            return value < 0 || value > 100;
        }
    }
}

