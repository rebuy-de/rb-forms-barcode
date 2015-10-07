using System;
using System.Collections.Generic;
using System.Linq;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using AndroidCamera = Android.Hardware.Camera;
using Android.Content;
using Android.Graphics;
using JObject = Java.Lang.Object;
using RebuyCameraSource = Com.Rebuy.Play.Services.Vision.CameraSource;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraConfigurator : JObject, ILog, RebuyCameraSource.IConfigurationCallback
    {
        private Configuration config;
        private Context context;
        private System.Drawing.Size viewSize;

        public CameraConfigurator SetContext(Context context)
        {
            this.context = context;
            return this;
        }

        public CameraConfigurator SetConfiguration(Configuration config)
        {
            this.config = config;
            return this;
        }

        public CameraConfigurator SetViewSize(System.Drawing.Size viewSize)
        {
            this.viewSize = viewSize;
            return this;
        }

        public AndroidCamera Configure(AndroidCamera camera)
        {
            var parameters = camera.GetParameters();

            camera.CancelAutoFocus();

            var focusMode = determineFocusMode(parameters);

            invokeIfAvailable(focusMode, val => {
                this.Debug("Focus Mode [{0}]", val);
                parameters.FocusMode = val;
            });

            var preview = determinePreviewResolution(parameters);
            this.Debug("Preview resolution [Width: {0}] x [Height {1}]", preview.Width, preview.Height);
            parameters.SetPreviewSize(preview.Width, preview.Height);

            var picture = determinePictureResolution(parameters, preview);
            this.Debug("Picture resolution [Width: {0}] x [Height {1}]", picture.Width, picture.Height);
            parameters.SetPictureSize(picture.Width, picture.Height);

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

            return camera;
        }

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

        private string determineFocusMode(AndroidCamera.Parameters p)
        {
            foreach (var mode in config.FocusModes) {
                if (p.SupportedFocusModes.Contains(mode)) {
                    return mode;
                }
            }

            return "";
        }

        private string determineSceneMode(AndroidCamera.Parameters parameters)
        {
            if (parameters.SupportedSceneModes.Contains(AndroidCamera.Parameters.SceneModeBarcode)) {
                return AndroidCamera.Parameters.SceneModeBarcode;
            }

            return AndroidCamera.Parameters.SceneModeAuto;
        }

        private string determineWhiteBalance(AndroidCamera.Parameters parameters)
        {
            if (parameters.SupportedWhiteBalance.Contains(AndroidCamera.Parameters.WhiteBalanceAuto)) {
                return AndroidCamera.Parameters.WhiteBalanceAuto;
            }

            return "";
        }

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

        private void invokeIfAvailable(string value, Action<string> cb)
        {
            if (string.IsNullOrEmpty(value)) {
                return;
            }

            cb.Invoke(value);
        }

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
                    var i = (int) Math.Ceiling(previewSizes.Count() / 2.0) - 1;
                    return previewSizes.ElementAt(i);
            }
        }

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
                .OrderByDescending(r => r.Width * r.Height)
                .DefaultIfEmpty(parameters.PictureSize)
                .First();
        }

        private double getViewRatio()
        {
            var ratio =  (double) viewSize.Width / (double) viewSize.Height;

            this.Debug("View [Ratio: {0}] based on [Width: {2}] / [Height {1}]", ratio, viewSize.Height, viewSize.Width);

            return ratio;
        }

    }
}
#pragma warning restore 618
