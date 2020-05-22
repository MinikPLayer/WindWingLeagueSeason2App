using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WindWingLeagueSeason2App.Services;
using WindWingLeagueSeason2App.Views;

using System.IO;

namespace WindWingLeagueSeason2App
{
    public partial class App : Application
    {
        

        public App()
        {
            InitializeComponent();

            //SetDynamicResource(DynamicBackgroundColor, "DarkSurface");


            //ThemeManager.SetLightMode();

            //ThemeManager.SetDarkMode();
            ThemeManager.darkMode = true;

            if(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt")))
            {
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt"));
            }

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
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
