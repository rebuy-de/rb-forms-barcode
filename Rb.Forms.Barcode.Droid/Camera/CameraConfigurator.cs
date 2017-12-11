using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Hardware;
using Rb.Forms.Barcode.Droid.Logger;
using Rb.Forms.Barcode.Pcl.Logger;
using AndroidCamera = Android.Hardware.Camera;
using JObject = Java.Lang.Object;
using RebuyCameraSource = Com.Rebuy.Play.Services.Vision.CameraSource;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraConfigurator : JObject, ILog, RebuyCameraSource.IConfigurationCallback
    {

        public class FpsRange
        {
            public uint Min { get; private set; }
            public uint Max { get; private set; }

            public FpsRange(int[] range)
            {
                Min = Convert.ToUInt32(range[(int) Preview.FpsMinIndex]);
                Max = Convert.ToUInt32(range[(int) Preview.FpsMaxIndex]);
            }

            public FpsRange(uint[] range)
            {
                Min = range[(int) Preview.FpsMinIndex];
                Max = range[(int) Preview.FpsMaxIndex];
            }

            public FpsRange(uint min, uint max)
            {
                Min = min;
                Max = max;
            }
        }
        private Configuration config;
        private System.Drawing.Size viewSize;

        /// <summary>
        /// Sets the camera configuration.
        /// </summary>
        public CameraConfigurator SetConfiguration(Configuration config)
        {
            this.config = config;
            return this;
        }

        /// <summary>
        /// Sets current view size.
        /// 
        /// This affects the selected camera preview size as we try to use a resolution which is
        /// within the bounds of the aspect ratio threshold.
        /// 
        /// <see cref="Configuration.PreviewResolution"/>
        /// <see cref="Configuration.AspectRatioThreshold" />
        /// </summary>
        public CameraConfigurator SetViewSize(System.Drawing.Size viewSize)
        {
            this.viewSize = viewSize;
            return this;
        }

        /// <summary>
        /// Configures the specified camera.
        /// </summary>
        public AndroidCamera Configure(AndroidCamera camera)
        {
            var parameters = camera.GetParameters();

            try {
                camera.CancelAutoFocus();
            } catch (Exception e) {
                this.Debug(e.Message);
            }

            parameters.PreviewFormat = ImageFormatType.Nv21;

            var focusMode = determineFocusMode(parameters);

            invokeIfAvailable(focusMode, val => {
                this.Debug("Focus Mode [{0}]", val);
                parameters.FocusMode = val;
            });

            var preview = determinePreviewResolution(parameters);
            this.Debug("Preview resolution [Width: {0}] x [Height {1}]", preview.Width, preview.Height);
            parameters.SetPreviewSize(preview.Width, preview.Height);

            // Set picture size with an equal ratio as the preview image.
            // According to google you will get a distorted image on some hardware, if you would only set the preview size.
            var picture = determinePictureResolution(parameters, preview);
            this.Debug("Picture resolution [Width: {0}] x [Height {1}]", picture.Width, picture.Height);
            parameters.SetPictureSize(picture.Width, picture.Height);


            if (config.TargetFps > 0) {
                var range = determineFpsRange(parameters, config.TargetFps);
                this.Debug("FPS Range [Min: {0}] - [Max {1}]", range.Min / 1000, range.Max / 1000);
                parameters.SetPreviewFpsRange((int) range.Min, (int) range.Max);
            }

            if (config.CompatibilityMode) {
                this.Debug("Compatibility mode enabled. Skipping advanced configuration to ensure highest compatibility.");
            }

            if (!config.CompatibilityMode) {
                if (config.SceneMode) {
                    var sceneMode = determineSceneMode(parameters);

                    invokeIfAvailable(sceneMode, val => {
                        this.Debug("Scene Mode [{0}]", val);
                        parameters.SceneMode = val;
                    });
                }

                if (config.MeteringAreas && parameters.MaxNumMeteringAreas > 0) {
                    this.Debug("Metering area [{0}]", (parameters.MaxNumMeteringAreas > 0).ToString());
                    parameters.MeteringAreas = createAreas();
                }

                if (config.FocusAreas && parameters.MaxNumFocusAreas > 0) {
                    this.Debug("Focusing area [{0}]", (parameters.MaxNumFocusAreas > 0).ToString());
                    parameters.FocusAreas = createAreas();
                }

                if (config.VideoStabilization && parameters.IsVideoStabilizationSupported) {
                    this.Debug("Video stabilization [{0}]", parameters.IsVideoStabilizationSupported.ToString());
                    parameters.VideoStabilization = true;
                }

                if (config.WhiteBalance) {
                    var whiteBalance = determineWhiteBalance(parameters);

                    invokeIfAvailable(whiteBalance, val => {
                        this.Debug("White balance [{0}]", val);
                        parameters.WhiteBalance = val;
                    });

                }
            }

            camera.SetParameters(parameters);

            if (config.Zoom > 0) {
                Zoom(camera, config.Zoom);
            }

            return camera;
        }

        /// <summary>
        /// Toggles the camera torch.
        /// If the camera does not support flash this is a noop.
        /// </summary>
        public void ToggleTorch(AndroidCamera camera, bool state) 
        {
            var parameters = camera.GetParameters();

            var flashMode = determineFlashMode(parameters, state);

            invokeIfAvailable(flashMode, val => {
                this.Debug("Flash mode [{0}]", val);
                parameters.FlashMode = val;
                camera.SetParameters(parameters);
            });
        }

        /// <summary>
        /// Apply camera zoom. The zoom value is capped to the maximum supported zoom value.
        /// If the camera does not support zoom this method is a noop.
        /// </summary>
        public void Zoom(AndroidCamera camera, uint zoom)
        {
            var parameters = camera.GetParameters();

            if (!parameters.IsZoomSupported) {
                return;
            }

            var cappedZoom = Convert.ToInt32(zoom);
            cappedZoom = (cappedZoom > parameters.MaxZoom) ? parameters.MaxZoom : cappedZoom;

            this.Debug("Zoom [{0}]", cappedZoom);
            parameters.Zoom = cappedZoom;

            camera.SetParameters(parameters);
        }

        /// <summary>
        /// Determines if the camera supports one of the requested focus modes in FIFO order.
        /// <see cref="Configuration.FocusModes" />
        /// </summary>
        private string determineFocusMode(AndroidCamera.Parameters parameters)
        {
            foreach (var mode in config.FocusModes) {
                if (parameters.SupportedFocusModes.Contains(mode)) {
                    return mode;
                }
            }

            return "";
        }

        /// <summary>
        /// Selects the fps range which is nearest to the desired target fps by calculating the diff between the
        /// target fps and the min and max range individually and then selecting the range with the lowest difference.
        /// If no matching fps range can be found, the currently active range will be returned.
        /// </summary>
        private FpsRange determineFpsRange(AndroidCamera.Parameters parameters, uint targetFps)
        {
            var ranges = parameters.SupportedPreviewFpsRange;

            if (ranges.Count == 1) {
                return new FpsRange(ranges.First());
            }

            Func<int[], long> fpsSorter = range =>  {
                var min = targetFps - range[(int) Preview.FpsMinIndex];
                var max = targetFps - range[(int) Preview.FpsMaxIndex];

                return Math.Abs(min) + Math.Abs(max);
            };

            var currentRange = new [] {0, 0};
            parameters.GetPreviewFpsRange(currentRange);

            var targetRange = ranges
                .OrderBy(fpsSorter)
                .DefaultIfEmpty(currentRange)
                .First();

            return new FpsRange(targetRange);
        }

        /// <summary>
        /// Determines if the camera supports the scene mode "Barcode".
        /// </summary>
        private string determineSceneMode(AndroidCamera.Parameters parameters)
        {
            if (parameters.SupportedSceneModes?.Contains(AndroidCamera.Parameters.SceneModeBarcode) ?? false) {
                return AndroidCamera.Parameters.SceneModeBarcode;
            }

            return AndroidCamera.Parameters.SceneModeAuto;
        }

        /// <summary>
        /// Determines if the camera supports the white balance mode "auto".
        /// </summary>
        private string determineWhiteBalance(AndroidCamera.Parameters parameters)
        {
            if (parameters.SupportedWhiteBalance.Contains(AndroidCamera.Parameters.WhiteBalanceAuto)) {
                return AndroidCamera.Parameters.WhiteBalanceAuto;
            }

            return "";
        }

        /// <summary>
        /// Determines if the camera supports flash modes that allow us to switch it on or off.
        /// </summary>
        private string determineFlashMode(AndroidCamera.Parameters p, bool state) 
        {
            if (state && p.SupportedFlashModes.Contains(AndroidCamera.Parameters.FlashModeTorch)) {
                return AndroidCamera.Parameters.FlashModeTorch;
            }

            if (state && p.SupportedFlashModes.Contains(AndroidCamera.Parameters.FlashModeOn)) {
                return AndroidCamera.Parameters.FlashModeOn;
            }

            if (!state && p.SupportedFlashModes.Contains(AndroidCamera.Parameters.FlashModeOff)) {
                return AndroidCamera.Parameters.FlashModeOff;
            }

            return "";
        }

        private List<AndroidCamera.Area> createAreas()
        {
            var area = new AndroidCamera.Area(new Rect(-1000, -400, 1000, 400), 1000);

            return new List<AndroidCamera.Area>() {
                area
            };
        }

        /// <summary>
        /// Invoke callback if value is not null or empty.
        /// </summary>
        private void invokeIfAvailable(string value, Action<string> cb)
        {
            if (string.IsNullOrEmpty(value)) {
                return;
            }

            cb.Invoke(value);
        }

        /// <summary>
        /// Filters preview sizes based on the current view dimensions by considering the aspect ratio.
        /// The preview size is then selected according the configured quality.
        /// 
        /// <see cref="Configuration.PreviewResolution"/>
        /// <see cref="Configuration.AspectRatioThreshold" />
        /// </summary>
        private AndroidCamera.Size determinePreviewResolution(AndroidCamera.Parameters parameters)
        {
            var referenceRatio = getViewRatio();

            /**
             * The aspect ratio between the preview resolution and the actual preview view
             * is not allowed to expand the configurated aspect ratio threshold.
             */
            Func<AndroidCamera.Size, bool> ratioFilter = preview =>  {
                var rawr = ((double) preview.Width / (double) preview.Height) - referenceRatio;

                return Math.Abs(rawr) <= config.AspectRatioThreshold;
            };

            /**
             * Remove all sizes that do not adhere to the desired aspect ratio 
             * and sort them by size so we can pick according to the desired quality.
             */
            var previewSizes = parameters.SupportedPreviewSizes
                .Where(ratioFilter)
                .OrderByDescending(preview => preview.Width * preview.Height)
                .DefaultIfEmpty(parameters.PreviewSize);

            switch (config.PreviewResolution) {
                case Configuration.Quality.High:
                    return previewSizes.First();

                case Configuration.Quality.Low:
                    return previewSizes.Last();

                case Configuration.Quality.Medium:
                default:
                    // when working with medium quality we always opt for the higher resolution.
                    var index = (int) Math.Ceiling(previewSizes.Count() / 2.0) - 1;
                    return previewSizes.ElementAt(index);
            }
        }

        /// <summary>
        /// Filters picture sizes based on the given preview size by considering the aspect ratio and the resolution.
        /// The first size in the remaining list is then selected.
        /// </summary>
        private AndroidCamera.Size determinePictureResolution(AndroidCamera.Parameters parameters, AndroidCamera.Size preview)
        {
            /**
             * The picture aspect ratio has to be as close as possible to the preview ratio.
             */
            Func<AndroidCamera.Size, bool> ratioFilter = picture =>  {
                var rawr = ((double) picture.Width / (double) picture.Height) - ((double) preview.Width / (double) preview.Height);

                return Math.Abs(rawr) <= 0.1;
            };

            return parameters.SupportedPictureSizes
                .Where(ratioFilter)
                // the picture resolution should not exceed the preview resolution
                .Where(picture => picture.Width * picture.Height <= preview.Width * preview.Height)
                .OrderByDescending(picture => picture.Width * picture.Height)
                .DefaultIfEmpty(parameters.PictureSize)
                .First();
        }

        /// <summary>
        /// Calculates the current view aspect ratio.
        /// </summary>
        private double getViewRatio()
        {
            var ratio =  (double) viewSize.Width / (double) viewSize.Height;

            this.Debug("View [Ratio: {0}] based on [Width: {2}] / [Height {1}]", ratio, viewSize.Height, viewSize.Width);

            return ratio;
        }

    }
}
#pragma warning restore 618
