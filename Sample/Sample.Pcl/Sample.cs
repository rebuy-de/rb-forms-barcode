using System;

using Xamarin.Forms;
using Sample.Pcl.Pages;
using System.Diagnostics;

namespace Sample.Pcl
{
    public class App : Application
    {
        public const string MessageOnStart = "OnStart";
        public const string MessageOnSleep = "OnSleep";
        public const string MessageOnResume = "OnResume";

        public App()
        {
            // The root page of your application
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            MessagingCenter.Send<App>(this, MessageOnStart);
        }

        protected override void OnSleep()
        {
            MessagingCenter.Send<App>(this, MessageOnSleep);
        }

        protected override void OnResume()
        {
            MessagingCenter.Send<App>(this, MessageOnResume);
        }
    }
}

