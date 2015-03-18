using System;

namespace Rb.Forms.Barcode.Pcl
{
   public class BarcodeEventArgs : EventArgs
   {
        public string Barcode { get; private set; }
        
        public BarcodeEventArgs(string barcode)
        {
            Barcode = barcode;
        }
   }
}
