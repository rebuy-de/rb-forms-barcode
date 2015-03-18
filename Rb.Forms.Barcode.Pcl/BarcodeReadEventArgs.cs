using System;

namespace Rb.Forms.Barcode.Pcl
{
   public class BarcodeReadEventArgs : BarcodeFoundEventArgs
   {
        public BarcodeReadEventArgs(string barcode) : base(barcode)
        {
        }
   }
}
