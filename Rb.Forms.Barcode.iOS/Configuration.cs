using System;
using System.Collections.Generic;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;

namespace Rb.Forms.Barcode.iOS
{
    public class Configuration
    {
		/// <summary>
		/// List of barcode types the decoder should look out for.
		/// Its recommended to narrow the list down to increase decoder performance.
		/// If no format is specified all available formats will be registered.
		/// </summary>
		public IList<RebuyBarcode.BarcodeFormat> Barcodes = new List<RebuyBarcode.BarcodeFormat>();
    }
}

