using System;
using System.Timers;
using Android.Hardware;
using Rb.Forms.Barcode.Droid.Logger;
using Rb.Forms.Barcode.Pcl.Logger;
using AndroidCamera = Android.Hardware.Camera;
using JObject = Java.Lang.Object;

#pragma warning disable 618
namespace Rb.Forms.Barcode.Droid.View
{
    public class AutoFocusCallback : JObject, AndroidCamera.IAutoFocusCallback, ILog
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

        public AutoFocusCallback(AndroidCamera camera)
        {
            timer = new Timer() {
                Interval = 400,
                Enabled = false,
                AutoReset = true
            };

            timer.Elapsed += (s, e) => {
                if (timer.Enabled) {
                    this.Debug("Autofocusing");
                    camera.AutoFocus(this);
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