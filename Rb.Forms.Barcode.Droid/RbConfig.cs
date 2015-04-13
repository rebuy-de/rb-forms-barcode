using System;
using System.Collections.Generic;
using AndroidCamera = Android.Hardware.Camera;

namespace Rb.Forms.Barcode.Droid
{
    public class RbConfig
    {
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
        /// Show performance related metrics in the application output.
        /// </summary>
        public bool Metrics = false;

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
    }
}

