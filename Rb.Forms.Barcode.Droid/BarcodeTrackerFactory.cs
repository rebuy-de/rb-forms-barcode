using System;
using Android.Gms.Vision;
using Rb.Forms.Barcode.Pcl;

namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeTrackerFactory : Java.Lang.Object, MultiProcessor.IFactory
    {
        private readonly BarcodeScanner barcodeScanner;

        public BarcodeTrackerFactory(BarcodeScanner barcodeScanner)
        {
            this.barcodeScanner = barcodeScanner;
        }

        public Tracker Create(Java.Lang.Object barcode)
        {
            return new BarcodeTracker(barcodeScanner);
        }
    }
}
