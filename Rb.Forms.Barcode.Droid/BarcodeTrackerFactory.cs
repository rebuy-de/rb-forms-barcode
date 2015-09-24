using System;
using Android.Gms.Vision;
using Rb.Forms.Barcode.Pcl;
using JObject = Java.Lang.Object;

namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeTrackerFactory : JObject, MultiProcessor.IFactory
    {
        private readonly BarcodeScanner barcodeScanner;

        public BarcodeTrackerFactory(BarcodeScanner barcodeScanner)
        {
            this.barcodeScanner = barcodeScanner;
        }

        public Tracker Create(JObject barcode)
        {
            return new BarcodeTracker(barcodeScanner);
        }
    }
}
