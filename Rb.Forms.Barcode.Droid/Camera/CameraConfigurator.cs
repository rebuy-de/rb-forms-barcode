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

        public void Configure(AndroidCamera camera)
        {
            var parameters = camera.GetParameters();

            camera.CancelAutoFocus();

            var focusMode = determineFocusMode(parameters);

            invokeIfAvailable(focusMode, val => {
                this.Debug("Focus Mode [{0}]", val);
                parameters.FocusMode = val;
            });

            camera.SetDisplayOrientation(90);

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
        }

        public void SetTorch(AndroidCamera camera, bool state) 
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


    }
}
#pragma warning restore 618
