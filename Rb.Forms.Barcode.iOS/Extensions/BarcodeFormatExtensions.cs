using System;
using AVFoundation;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;
using System.Collections.Generic;

namespace Rb.Forms.Barcode.iOS.Extensions
{    
    public static class BarcodeFormatExtensions 
    {
        public static AVMetadataObjectType ConvertToIOS(this IList<RebuyBarcode.BarcodeFormat> barcodeFormat) 
        {
            AVMetadataObjectType types = AVMetadataObjectType.None;

            var mapping = new Dictionary<RebuyBarcode.BarcodeFormat, AVMetadataObjectType> {
                { RebuyBarcode.BarcodeFormat.Code128, AVMetadataObjectType.Code128Code },
                { RebuyBarcode.BarcodeFormat.Code39, AVMetadataObjectType.Code39Code},
                { RebuyBarcode.BarcodeFormat.Code93, AVMetadataObjectType.Code93Code },
                { RebuyBarcode.BarcodeFormat.DataMatrix, AVMetadataObjectType.DataMatrixCode },
                { RebuyBarcode.BarcodeFormat.Ean13,AVMetadataObjectType.EAN13Code },
                { RebuyBarcode.BarcodeFormat.Ean8, AVMetadataObjectType.EAN8Code },
                { RebuyBarcode.BarcodeFormat.Itf, AVMetadataObjectType.ITF14Code },
                { RebuyBarcode.BarcodeFormat.Pdf417, AVMetadataObjectType.PDF417Code },
                { RebuyBarcode.BarcodeFormat.QrCode, AVMetadataObjectType.QRCode },
                { RebuyBarcode.BarcodeFormat.UpcE, AVMetadataObjectType.UPCECode },
            };

            foreach (RebuyBarcode.BarcodeFormat barcode in barcodeFormat) {
                if (mapping.ContainsKey(barcode)) {
                    types |= mapping[barcode];
                }
            }

            return types;
        }

        public static RebuyBarcode.BarcodeFormat ConvertToPcl(this AVMetadataObjectType type) 
        {
            var mapping = new Dictionary<AVMetadataObjectType, RebuyBarcode.BarcodeFormat> {
                { AVMetadataObjectType.Code128Code, RebuyBarcode.BarcodeFormat.Code128 },
                { AVMetadataObjectType.Code39Code, RebuyBarcode.BarcodeFormat.Code39 },
                { AVMetadataObjectType.Code93Code, RebuyBarcode.BarcodeFormat.Code93 },
                { AVMetadataObjectType.DataMatrixCode, RebuyBarcode.BarcodeFormat.DataMatrix },
                { AVMetadataObjectType.EAN13Code, RebuyBarcode.BarcodeFormat.Ean13 },
                { AVMetadataObjectType.EAN8Code, RebuyBarcode.BarcodeFormat.Ean8 },
                { AVMetadataObjectType.ITF14Code, RebuyBarcode.BarcodeFormat.Itf },
                { AVMetadataObjectType.PDF417Code, RebuyBarcode.BarcodeFormat.Pdf417 },
                { AVMetadataObjectType.QRCode, RebuyBarcode.BarcodeFormat.QrCode },
                { AVMetadataObjectType.UPCECode, RebuyBarcode.BarcodeFormat.UpcE },
            };

            return !mapping.ContainsKey(type) ? RebuyBarcode.BarcodeFormat.Unknown : mapping[type];       
        }
    }
}
