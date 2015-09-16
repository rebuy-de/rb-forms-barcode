using System;
using System.Collections.Generic;
using System.Linq;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using AndroidCamera = Android.Hardware.Camera;
using Android.Content;
using Android.Graphics;
using Android.Gms.Vision;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraConfigurator : ILog
    {
        private readonly Configuration config;
        private readonly Context context;

        public CameraConfigurator(Configuration config, Context context)
        {
            this.context = context;
            this.config = config;
        }

        public void Configure(CameraSource cameraSource)
        {
            Configure(cameraSource.GetCamera());
        }

        public void Configure(AndroidCamera camera)
        {
            var parameters = camera.GetParameters();

            camera.CancelAutoFocus();

            var focusMode = determineFocusMode(parameters);
            this.Debug("Focus Mode [{0}]", focusMode);
            parameters.FocusMode = focusMode;

            camera.SetDisplayOrientation(90);

            if (config.CompatibilityMode) {
                this.Debug("Compatibility mode enabled. Skipping advanced configuration to ensure highest compatibility.");
            }

            if (!config.CompatibilityMode) {
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

        private List<AndroidCamera.Area> createAreas()
        {
            var area = new AndroidCamera.Area(new Rect(-1000, -400, 1000, 400), 1000);

            return new List<AndroidCamera.Area>() {
                area
            };
        }
    }
}
#pragma warning restore 618
