using System;
using Android.Gms.Vision.Barcodes;

namespace Rb.Forms.Barcode.Droid
{
    public class BarcodeTrackerFactory : Java.Lang.Object, Android.Gms.Vision.MultiProcessor.IFactory
    {
        public Android.Gms.Vision.Tracker Create(Java.Lang.Object barcode)
        {
            return new MyBarcodeTracker();

        }

//        private GraphicOverlay mGraphicOverlay;
//
//        BarcodeTrackerFactory(GraphicOverlay graphicOverlay) {
//            mGraphicOverlay = graphicOverlay;
//        }

    }

    class MyBarcodeTracker : Android.Gms.Vision.Tracker {

        public override void OnNewItem(int idValue, Java.Lang.Object item)
        {
            base.OnNewItem(idValue, item);
        }

        public override void OnUpdate(Android.Gms.Vision.Detector.Detections detections, Java.Lang.Object item)
        {
            base.OnUpdate(detections, item);
        }
    }
}

