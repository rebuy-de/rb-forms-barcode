using System;
using System.Collections.Generic;
using AndroidCamera = Android.Hardware.Camera;

namespace Rb.Forms.Barcode.Droid
{
    public class RbConfig
    {

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
            AndroidCamera.Parameters.FocusModeContinuousVideo,
            AndroidCamera.Parameters.FocusModeContinuousPicture,
            AndroidCamera.Parameters.FocusModeAuto,
        };

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

