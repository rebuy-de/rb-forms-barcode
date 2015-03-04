using System;

using Xamarin.Forms;
using System.Diagnostics;

namespace Rb.Forms.Barcode.Pcl
{
    public class BarcodeScanner : View
   {
        public static readonly BindableProperty BarcodeProperty =
            BindableProperty.Create<BarcodeScanner, String>(
                p => p.Barcode, default(string), BindingMode.OneWayToSource);

        public String Barcode
        {
            get { return (String) GetValue(BarcodeProperty); }
            set { SetValue(BarcodeProperty, value); }
        }

        public event EventHandler<BarcodeFoundEventArgs> BarcodeFound;

        public void RaiseBarcodeFoundEvent(String barcode)
        {
            Barcode = barcode;

            Debug.WriteLine("[ScannerView] RaiseBarcodeFoundEvent [{0}]", barcode, null);
            if (null == this.BarcodeFound)  {
                return;
            }

            this.BarcodeFound(this, new BarcodeFoundEventArgs(barcode));
        }
    }   
}
