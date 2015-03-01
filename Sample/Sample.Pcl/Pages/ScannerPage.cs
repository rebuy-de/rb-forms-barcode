using System;
using Rb.Forms.Barcode.Pcl;
using Xamarin.Forms;

namespace Sample.Pcl.Pages
{
    public class ScannerPage : ContentPage
    {
        private readonly BarcodeScanner barcodeScanner = new BarcodeScanner();

        public ScannerPage()
        {
            NavigationPage.SetHasBackButton(this, false);

            var relativeLayout = new RelativeLayout();
            relativeLayout.Children.Add(
                barcodeScanner,
                widthConstraint: Constraint.RelativeToParent ((parent) => { return parent.Width; }),
                heightConstraint: Constraint.RelativeToParent ((parent) => { return parent.Height; })
            );

            relativeLayout.Children.Add(
                new BoxView() { Color = Color.Transparent},
                widthConstraint: Constraint.RelativeToParent ((parent) => { return parent.Width; }),
                heightConstraint: Constraint.RelativeToParent ((parent) => { return parent.Height; })
            );

            var sl = new StackLayout() {
                Padding = 20,
                Children = {
                    new Label { Text = "Hold the screen above a barcode." },
                }
            };

            relativeLayout.Children.Add(
                sl,
                Constraint.RelativeToParent ((parent) => { return 0; })
            );

            Content = relativeLayout;
        }

        ~ScannerPage ()
        {
            barcodeScanner.IsEnabled = false;
        }
    }
}

