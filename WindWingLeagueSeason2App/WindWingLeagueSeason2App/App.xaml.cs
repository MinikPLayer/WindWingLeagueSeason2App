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
#if DEBUG
        public const bool debug = true;
#else
        public const bool debug = false;
#endif


        public const int serverSupportVersion = 5;
        public const int appVersion = 700;

        public App()
        {
            InitializeComponent();

            ThemeManager.darkMode = true;

            if(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt")))
            {
                File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt"), "[" + DateTime.Now.ToString() + "]\n");
            }

            DependencyService.Register<MockDataStore>();
            if(MainPage == null)
            {
                MainPage = new MainPage();
            }
            
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
