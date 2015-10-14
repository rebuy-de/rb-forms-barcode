using System;
using Sample.Pcl.Pages;


namespace Sample.Pcl
{
    public class ScannerPageControl
    {
        private static readonly Lazy<ScannerPageControl> lazy =
            new Lazy<ScannerPageControl>(() => new ScannerPageControl());

        public static ScannerPageControl Instance { get { return lazy.Value; } }

        private ScannerPage scannerPage;

        private ScannerPageControl()
        {
        }

        public ScannerPage CreateScannerPage()
        {
            scannerPage?.DisableScanner();
            scannerPage = new ScannerPage();

            return scannerPage;
        }
    }
}


