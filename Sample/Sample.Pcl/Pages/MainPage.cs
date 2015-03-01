using System;
using Xamarin.Forms;

namespace Sample.Pcl.Pages
{
    public class MainPage : MasterDetailPage
    {
        public MainPage()
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

            scannerPageButton.Clicked += (sender, e) => {
                Detail.Navigation.PushAsync(new ScannerPage());
                IsPresented = false;
            };

            Master = new ContentPage {
                Title = "Master",
                Content = new StackLayout {
                    Children = {
                        mainPageButton,
                        scannerPageButton
                    }
                },
            };

            Detail = new NavigationPage(new ContentPage {
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            XAlign = TextAlignment.Center,
                            Text = "Use the menu to go to the surface page."
                        }
                    }
                }
            });
        }
    }
}

