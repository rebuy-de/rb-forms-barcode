using System;
using System.Collections.Generic;
using AndroidCamera = Android.Hardware.Camera;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;

namespace Rb.Forms.Barcode.Droid
{
    public class Configuration
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
            AndroidCamera.Parameters.FocusModeContinuousPicture,
            AndroidCamera.Parameters.FocusModeContinuousVideo,
            AndroidCamera.Parameters.FocusModeAuto,
        };

        /// <summary>
        /// List of barcode types the decoder should look out for.
        /// Its recommended to narrow the list down to increase decoder performance.
        /// If no format is specified all available formats will be registered.
        /// </summary>
        public IList<RebuyBarcode.BarcodeFormat> Barcodes = new List<RebuyBarcode.BarcodeFormat>();


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
    }
}

