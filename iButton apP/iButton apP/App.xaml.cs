using iButton_apP.Services;
using iButton_apP.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace iButton_apP
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
