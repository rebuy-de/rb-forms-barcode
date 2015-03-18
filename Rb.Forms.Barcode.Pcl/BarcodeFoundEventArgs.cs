using System;

namespace Rb.Forms.Barcode.Pcl
{
   public class BarcodeFoundEventArgs : EventArgs
   {
        public string Barcode { get; private set; }
        
        public BarcodeFoundEventArgs(string barcode)
        {
            Barcode = barcode;
        }
   }
}
