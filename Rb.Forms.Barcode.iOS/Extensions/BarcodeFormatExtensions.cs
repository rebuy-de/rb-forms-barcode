using System;
using AVFoundation;
using RebuyBarcode = Rb.Forms.Barcode.Pcl.Barcode;
using System.Collections.Generic;

namespace Rb.Forms.Barcode.iOS.Extensions
{    
	public static class BarcodeFormatExtensions 
	{
		public static AVMetadataObjectType ConvertToIOS(this RebuyBarcode.BarcodeFormat barcodeFormat) 
		{
			AVMetadataObjectType types = AVMetadataObjectType.None;

			foreach (RebuyBarcode.BarcodeFormat barcode in GetFlags(barcodeFormat)) {
				switch (barcode) {
					case RebuyBarcode.BarcodeFormat.Code39: 
						types |= AVMetadataObjectType.Code39Code;
						break;
					case RebuyBarcode.BarcodeFormat.Code93: 
						types |= AVMetadataObjectType.Code93Code;
						break;
					case RebuyBarcode.BarcodeFormat.Code128: 
						types |= AVMetadataObjectType.Code128Code;
						break;
					case RebuyBarcode.BarcodeFormat.DataMatrix: 
						types |= AVMetadataObjectType.DataMatrixCode;
						break;
					case RebuyBarcode.BarcodeFormat.Ean13: 
						types |= AVMetadataObjectType.EAN13Code;
						break;
					case RebuyBarcode.BarcodeFormat.Ean8: 
						types |= AVMetadataObjectType.EAN8Code;
						break;
					case RebuyBarcode.BarcodeFormat.Itf: 
						types |= AVMetadataObjectType.ITF14Code;
						break;
					case RebuyBarcode.BarcodeFormat.Pdf417: 
						types |= AVMetadataObjectType.PDF417Code;
						break;
					case RebuyBarcode.BarcodeFormat.QrCode: 
						types |= AVMetadataObjectType.QRCode;
						break;
					case RebuyBarcode.BarcodeFormat.UpcE: 
						types |= AVMetadataObjectType.UPCECode;
						break;
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

			if (!mapping.ContainsKey(type)) {
				return RebuyBarcode.BarcodeFormat.Unknown;
			}		

			return mapping[type];
		}

		private static IEnumerable<Enum> GetFlags(Enum input)
		{
			foreach (Enum value in Enum.GetValues(input.GetType())) {
				if (input.HasFlag(value)) {
					yield return value;
				}
			}
		}


	}
}