using System;
using System.Collections.Generic;
using AndroidCamera = Android.Hardware.Camera;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;

namespace Rb.Forms.Barcode.Droid
{
    public class Configuration
    {
        public enum Quality {
            High,
            Medium,
            Low
        };

        /// <summary>
        /// Enable or disable compatible mode.
        /// If enabled advanced camera features, except focus mode configuration,
        /// wont be activated to ensure the highest device compatibility.
        /// </summary>
        public bool CompatibilityMode = true;

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
        /// Defines the preview resolution quality. Higher quality allows the detector
        /// to reaed smaller barcodes at longer distances.
        /// 
        /// The default device dependant preview resolution will be used as fallback.
        /// </summary>
        /// <remarks>
        /// The aspect ratio of the barcode view is considered when selecting a preview resolution.
        /// <seealso cref="Configuration.AspectRatioThreshold" />
        /// </remarks>
        public Quality PreviewResolution = Quality.High;

        /// <summary>
        /// Threshold value that controls if a preview resolution should be discarded when the aspect ratio exceeds
        /// the view ratio.
        /// A bigger tolerance level means more resolutions to pick from but can result in distorted preview images.
        /// <seealso cref="Configuration.PreviewResolution"/>
        /// </summary>
        public double AspectRatioThreshold = 0.8;

        /// <summary>
        /// Enable or disable metering area configuration.
        /// If false no metering areas will be set even if the device claims to support it.
        /// </summary>
        /// <remarks>
        /// Advanced device feature, disabled when compatibility mode is turned on.
        /// </remarks>
        public bool MeteringAreas = true;

        /// <summary>
        /// Enable or disable focus area configuration.
        /// If false no focus areas will be set even if the device claims to support it.
        /// </summary>
        /// <remarks>
        /// Advanced device feature, disabled when compatibility mode is turned on.
        /// </remarks>
        public bool FocusAreas = true;

        /// <summary>
        /// Enable or disable scene mode configuration.
        /// If false no scene mode will be set even if the device claims to support it.
        /// </summary>
        /// <remarks>
        /// Advanced device feature, disabled when compatibility mode is turned on.
        /// </remarks>
        public bool SceneMode = true;

        /// <summary>
        /// Enable or disable video stabilization configuration.
        /// If false video stabilization will not be enabled even if the device claims to support it.
        /// </summary>
        /// <remarks>
        /// Advanced device feature, disabled when compatibility mode is turned on.
        /// </remarks>
        public bool VideoStabilization = true;

        /// <summary>
        /// Enable or disable auto whitebalance configuration.
        /// If false auto whitebalance will not be enabled even if the device claims to support it.
        /// </summary>
        /// <remarks>
        /// Advanced device feature, disabled when compatibility mode is turned on.
        /// </remarks>
        public bool WhiteBalance = true;
    }
}

