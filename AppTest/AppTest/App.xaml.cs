using Xamarin.Forms;
using AppTest.Views;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Xamarin.Essentials;
using System;
using System.Net.Http;
using System.Collections.Generic;

namespace AppTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // This should come before AppCenter.Start() is called
            // Avoid duplicate event registration:
            if (!AppCenter.Configured)
            {
                try
                {
                    Push.PushNotificationReceived += async (sender, e) =>
                    {
                        Preferences.Set("PushReceived", DateTime.Now);
                        Analytics.TrackEvent("PushReceived", e.CustomData);

                        var httpClient = new HttpClient();
                        var googleHTML = await httpClient.GetStringAsync("https://google.com");

                        Preferences.Set("DownloadedData", googleHTML.Substring(0, 20));

                        Analytics.TrackEvent("PushHandled", new Dictionary<string, string>()
                        {
                            { "Downloaded", googleHTML }
                        });
                    };
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }

            AppCenter.Start("ios=756f3428-45b9-4795-9fbb-f8f22d824f3f;",
                              typeof(Analytics), 
                              typeof(Crashes), 
                              typeof(Push));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
