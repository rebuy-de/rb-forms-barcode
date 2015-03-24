using System;
using System.Timers;
using Rb.Forms.Barcode.Pcl.Logger;
using Rb.Forms.Barcode.Droid.Logger;

using Android.Hardware;
using AndroidCamera = Android.Hardware.Camera;
using Rb.Forms.Barcode.Droid.Camera;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.View
{
    public class AutoFocusCallback : Java.Lang.Object, AndroidCamera.IAutoFocusCallback, ILog
    {
        private Timer timer;

        public bool Enabled {
            get {
                return timer.Enabled;
            }
            set {
                timer.Enabled = value;
            }
        }

        public AutoFocusCallback(ScannerCamera scannerCamera)
        {
            timer = new Timer() {
                Interval = 400,
                Enabled = false,
                AutoReset = true
            };

            timer.Elapsed += (s, e) => {
                if (timer.Enabled) {

                    if (!scannerCamera.CameraOpen) {
                        timer.Stop();
                        return;
                    }

                    if (!scannerCamera.AutoFocusMode) {
                        timer.Stop();
                        return;
                    }

                    this.Debug("Autofocusing");
                    scannerCamera.AutoFocus(this);
                }
            };

        }

        public void OnAutoFocus(bool success, AndroidCamera camera)
        {
            this.Debug("OnAutoFocus");

            try {
                timer.Start();
            } catch (Exception ex) {
                this.Debug(ex.ToString());
            }
        }
    }
}

#pragma warning restore 618