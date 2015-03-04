using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Sample.Pcl;

namespace Sample.Pcl.Pages
{
    public class MainPage : MasterDetailPage
    {
        public MainPage()
        {

            Master = createMaster();
            Detail = createDetail();
        }

        private Page createMaster()
        {
            var mainPageButton = new Button() {
                Text = "Mainpage"
            };

            mainPageButton.Clicked += (sender, e) => {
                Detail.Navigation.PopToRootAsync();
                IsPresented = false;
            };

            var scannerPageButton = new Button() {
                Text = "Scanner",
            };

            scannerPageButton.Clicked += gotoScanner;

            return new ContentPage {
                Title = "Master",
                Content = new StackLayout {
                    Children = {
                        mainPageButton,
                        scannerPageButton
                    }
                },
            };
        }

        private Page createDetail()
        {

            var scanner = new Button {
                Text = "Scan all the things!"
            };

            scanner.Clicked += gotoScanner;

            return new NavigationPage(new ContentPage {
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Padding = 20,
                    Children = {
                        new Label {
                            Text = "Use the menu or the button below to see the BarcodeScanner in action."
                        },
                        new Label {
                            Text = "Please report any issues or feature requests at https://github.com/rebuy-de/rb-forms-barcode"
                        },
                        scanner
                    }
                }
            });
        }

        private void gotoScanner(Object sender, EventArgs e)
        {
            Detail.Navigation.PopToRootAsync();
            Detail.Navigation.PushAsync(new ScannerPage());
            IsPresented = false;
        }

    }
}

