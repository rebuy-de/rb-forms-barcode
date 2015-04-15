using System;
using System.Collections.Generic;
using System.Linq;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using AndroidCamera = Android.Hardware.Camera;
using Android.Content;
using Android.Views;
using Android.Util;
using Android.Runtime;
using Android.Graphics;
using ApxLabs.FastAndroidCamera;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraConfigurator : ILog
    {
        private readonly RbConfig config;
        private readonly Context context;

        public CameraConfigurator(RbConfig config, Context context)
        {
            this.context = context;
            this.config = config;
            
        }

        public AndroidCamera Configure(AndroidCamera camera)
        {
            var parameters = camera.GetParameters();

            camera.CancelAutoFocus();

            var focusMode = determineFocusMode(parameters);
            this.Debug("Focus Mode [{0}]", focusMode);
            parameters.FocusMode = focusMode;


            camera.SetDisplayOrientation(90);

            var resolution = determineResolution(parameters);
            this.Debug("Preview resolution [Width: {0}] x [Height {1}]", resolution.Width, resolution.Height);
            parameters.SetPreviewSize(resolution.Width, resolution.Height);

            var buffersize = calculateBufferSize(parameters);

            for (int i = 0; i <= 3; i++) {
                camera.AddCallbackBuffer(new FastJavaByteArray(buffersize));
            }

            if (isPickyDevice()) {
                this.Debug("Used device is marked as picky. Skipping detailed configuration to ensure function compatibility.");
            }

            if (!isPickyDevice()) {
                if (config.SceneMode) {
                    var sceneMode = determineSceneMode(parameters);
                    this.Debug("Scene Mode [{0}]", sceneMode);
                    parameters.SceneMode = sceneMode;
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
                    this.Debug("White balance [{0}]", whiteBalance);
                    parameters.WhiteBalance = whiteBalance;
                }
            }

            camera.SetParameters(parameters);

            return camera;
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

        private AndroidCamera.Size determineResolution(AndroidCamera.Parameters parameters)
        {
            var referenceRatio = getDisplayRatio();

            Func<AndroidCamera.Size, bool> ratioFilter = (r) =>  {
                var rawr = ((double) r.Width / (double) r.Height) - referenceRatio;
                return Math.Abs(rawr) <= config.AspectRatioThreshold;
            };

            var previewSizes = parameters.SupportedPreviewSizes
                .Where(ratioFilter)
                .OrderByDescending(r => r.Width * r.Height)
                .DefaultIfEmpty(parameters.PreviewSize);

            switch (config.PreviewResolution) {
                case RbConfig.Quality.High:
                    return previewSizes.First();

                case RbConfig.Quality.Low:
                    return previewSizes.Last();

                case RbConfig.Quality.Medium:
                default:
                    var i = (int) Math.Ceiling(previewSizes.Count() / 2.0) - 1;
                    return previewSizes.ElementAt(i);
            }
        }

        private double getDisplayRatio()
        {
            var displayMetrics = new DisplayMetrics();

            var windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            windowManager.DefaultDisplay.GetMetrics(displayMetrics);

            return (double) displayMetrics.HeightPixels / (double) displayMetrics.WidthPixels;
        }

        private int calculateBufferSize(AndroidCamera.Parameters parameters)
        {
            return parameters.PreviewSize.Width * parameters.PreviewSize.Height * ImageFormat.GetBitsPerPixel(parameters.PreviewFormat) / 8;
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

        private List<AndroidCamera.Area> createAreas()
        {
            var area = new AndroidCamera.Area(new Rect(-1000, -400, 1000, 400), 1000);

            return new List<AndroidCamera.Area>() {
                area
            };
        }

        private bool isPickyDevice()
        {
            return config.PickyDeviceDetection && Android.OS.Build.Manufacturer.Contains("samsung");
        }
    }
}
#pragma warning restore 618