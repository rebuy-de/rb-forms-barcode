using System;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Content;
using Rb.Forms.Barcode.Pcl;
using Rb.Forms.Barcode.Droid.Camera;
using Rb.Forms.Barcode.Droid.View;
using Com.Rebuy.Play.Services.Vision;
using RebuyCameraSource = Com.Rebuy.Play.Services.Vision.CameraSource;

namespace Rb.Forms.Barcode.Droid.Camera
{
    public class CameraServiceFactory
    {
        private readonly CameraConfigurator configurator;

        public CameraServiceFactory(CameraConfigurator configurator)
        {
            this.configurator = configurator;
        }

        public CameraService Create(Context context, BarcodeScanner barcodeScanner)
        {
            var cs = createCameraSource(context, barcodeScanner);

            return new CameraService(barcodeScanner, cs, configurator);
        }

        private RebuyCameraSource createCameraSource(Context context, BarcodeScanner barcodeScanner)
        {
            var barcodeDetector = new BarcodeDetector.Builder(context).Build();
            var barcodeFactory = new BarcodeTrackerFactory(barcodeScanner);
            barcodeDetector.SetProcessor(new MultiProcessor.Builder(barcodeFactory).Build());

            return new RebuyCameraSource.Builder(context, barcodeDetector)
                .SetFacing(RebuyCameraSource.CameraFacingBack)
                .SetConfigurator(configurator)
                .Build();

        }
    }
}

