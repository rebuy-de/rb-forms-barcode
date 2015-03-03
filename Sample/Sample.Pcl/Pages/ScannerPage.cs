using System;
using Rb.Forms.Barcode.Pcl;
using Xamarin.Forms;

namespace Sample.Pcl.Pages
{
    public class ScannerPage : ContentPage
    {
        private readonly BarcodeScanner barcodeScanner = new BarcodeScanner();

        private readonly BoxView zeLine = new BoxView() { Color = Color.Red, HeightRequest = 1 };

        private readonly Label result = new Label {
            TextColor = Color.Red,
            FontSize = 17.0,
            XAlign = TextAlignment.Center
        };

        private readonly BoxView flash = new BoxView() {
            Color = Color.White,
            Opacity = 0
        };

        private readonly RelativeLayout relativeLayout = new RelativeLayout();

        private readonly StackLayout stackLayout = new StackLayout() {
            Padding = 20
        };

        public ScannerPage()
        {
            NavigationPage.SetHasBackButton(this, false);

            /**
             * Event that gets executed as soon as a barcode is detected.
             */
            barcodeScanner.BarcodeFound += (object sender, BarcodeFoundEventArgs e) => {
                flashScreenAsync(sender, e);
                result.Text = String.Format("Last Barcode: {0}", e.Barcode);
            };

            /**
             * So that we can release the camera when turning off phone or switching apps.
             */
            MessagingCenter.Subscribe<App>(this, App.MessageOnSleep, onSleep);
            MessagingCenter.Subscribe<App>(this, App.MessageOnResume, onResume);

            Content = createViewLayout();
        }

        private View createViewLayout()
        {
            /**
             * Add the scanner itself to the view.
             */
            addExpandingViewToRelativeLayout(barcodeScanner);

            /**
             * A transparent box is required so that overlaying elements are visible.
             */
            addExpandingViewToRelativeLayout(new BoxView() { Color = Color.Transparent});

            /**
             * Awesome barcode scanner line.
             */
            relativeLayout.Children.Add(
                zeLine,
                widthConstraint: Constraint.RelativeToParent ((parent) => { return parent.Width - 100; }),
                yConstraint: Constraint.RelativeToParent ((parent) => { return parent.Height / 2; }),
                xConstraint: Constraint.RelativeToParent ((parent) => { return 50; })
            );

            /**
             * Result output below scanner line
             */
            relativeLayout.Children.Add(
                result,
                widthConstraint: Constraint.RelativeToParent ((parent) => { return parent.Width; }),
                yConstraint: Constraint.RelativeToView (zeLine, (parent, sibling) => { return sibling.Y + 10; })
            );

            /**
             * Screen flash.
             */
            addExpandingViewToRelativeLayout(flash);

            /**
             * Just some usage explanation. Bork Bork.
             */
            stackLayout.Children.Add(new Label {
                Text = "Hold the scanner above a barcode, wait for the autofocus and see its magic!",
                XAlign = TextAlignment.Center
            });

            relativeLayout.Children.Add(
                stackLayout,
                Constraint.RelativeToParent ((parent) => { return 0; })
            );

            return relativeLayout;
        }

        /**
         * Release camera so that other apps can access it.
         */
        private void onSleep(object sender) {
            barcodeScanner.IsEnabled = false;
        }

        /**
         * All your camera belongs to us.
         */
        private void onResume(object sender) {
            barcodeScanner.IsEnabled = true;
        }

        private void addExpandingViewToRelativeLayout(View view)
        {
            relativeLayout.Children.Add(
                view,
                widthConstraint: Constraint.RelativeToParent ((parent) => { return parent.Width; }),
                heightConstraint: Constraint.RelativeToParent ((parent) => { return parent.Height; })
            );
        }

        async private void flashScreenAsync(Object sender, EventArgs e)
        {
            await flash.FadeTo(1, 150, Easing.CubicIn);
            await flash.FadeTo(0, 150, Easing.CubicOut);
        }

        /**
         * You need to take care of realeasing the camera when you are done with it else bad things can happen!
         */
        ~ScannerPage ()
        {
            barcodeScanner.IsEnabled = false;

            /**
             * Camera is released we dont need the events anymore.
             */
            MessagingCenter.Unsubscribe<App>(this, App.MessageOnSleep);
            MessagingCenter.Unsubscribe<App>(this, App.MessageOnResume);
        }
    }
}

