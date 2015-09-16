using System;
using Rb.Forms.Barcode.Pcl;
using Android.Runtime;
using Android.Gms.Vision;
using AndroidBarcode = Android.Gms.Vision.Barcodes.Barcode;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;
using System.Collections.Generic;
using Android.Gms.Vision.Barcodes;

namespace Rb.Forms.Barcode.Droid
{
    class BarcodeTracker : Tracker
    {
        private readonly BarcodeScanner barcodeScanner;

        private readonly Dictionary<BarcodeFormat, RebuyBarcode.BarcodeFormat> formatMapping =
            new Dictionary<BarcodeFormat, RebuyBarcode.BarcodeFormat> {
                { BarcodeFormat.Codabar, RebuyBarcode.BarcodeFormat.Codabar },
                { BarcodeFormat.Code128, RebuyBarcode.BarcodeFormat.Code128 },
                { BarcodeFormat.Code39, RebuyBarcode.BarcodeFormat.Code39 },
                { BarcodeFormat.Code93, RebuyBarcode.BarcodeFormat.Code93 },
                { BarcodeFormat.DataMatrix, RebuyBarcode.BarcodeFormat.DataMatrix },
                { BarcodeFormat.Ean13, RebuyBarcode.BarcodeFormat.Ean13 },
                { BarcodeFormat.Ean8, RebuyBarcode.BarcodeFormat.Ean8 },
                { BarcodeFormat.Itf, RebuyBarcode.BarcodeFormat.Itf },
                { BarcodeFormat.Pdf417, RebuyBarcode.BarcodeFormat.Pdf417 },
                { BarcodeFormat.QrCode, RebuyBarcode.BarcodeFormat.QrCode },
                { BarcodeFormat.UpcA, RebuyBarcode.BarcodeFormat.UpcA },
                { BarcodeFormat.UpcE, RebuyBarcode.BarcodeFormat.UpcE },
            };

        public BarcodeTracker(BarcodeScanner barcodeScanner)
        {
            this.barcodeScanner = barcodeScanner;
        }

        public override void OnNewItem(int idValue, Java.Lang.Object item)
        {
            barcodeScanner.Barcode = createResult(item);
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
            barcodeScanner.Barcode = createResult(item);
        }

        private RebuyBarcode createResult(Java.Lang.Object item)
        {
            var barcode = item.JavaCast<AndroidBarcode>();
            return new RebuyBarcode(barcode.DisplayValue, convertFormat(barcode));
        }

        private RebuyBarcode.BarcodeFormat convertFormat(AndroidBarcode barcode)
        {
            if (!formatMapping.ContainsKey(barcode.Format)) {
                return RebuyBarcode.BarcodeFormat.Unknown;
            }

            return formatMapping[barcode.Format];
        }
    }
}

