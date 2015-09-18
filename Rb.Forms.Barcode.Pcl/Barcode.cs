using System;

namespace Rb.Forms.Barcode.Pcl
{
    public class Barcode
    {
        public enum BarcodeFormat
        {
            Unknown = 0,
            Code128 = 1,
            Code39 = 2,
            Code93 = 4,
            Codabar = 8,
            DataMatrix = 16,
            Ean13 = 32,
            Ean8 = 64,
            Itf = 128,
            QrCode = 256,
            UpcA = 512,
            UpcE = 1024,
            Pdf417 = 2048
        }

        public BarcodeFormat Format { get; private set; }
        public String Result { get; private set; }

        public Barcode(String result, BarcodeFormat format)
        {
            this.Result = result;
            this.Format = format;
        }

        public override bool Equals(object obj)
        {
            Barcode barcode = obj as Barcode;

            if (barcode == null) {
                return false;
            }

            if (ReferenceEquals(this, barcode)) {
                return true;
            }

            if (barcode.GetType() != typeof(Barcode)) {
                return false;
            }

            return Format == barcode.Format && Result == barcode.Result;
        }
        

        public override int GetHashCode()
        {
            unchecked {
                return Format.GetHashCode() ^ (Result != null ? Result.GetHashCode() : 0);
            }
        }
        
    }
}

