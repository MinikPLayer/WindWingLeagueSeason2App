using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.IO;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class OptionsPage : ContentPage
    {
        bool initialized = false;
        public OptionsPage()
        {
            InitializeComponent();

            Title = "Opcje";
        }

        protected override void OnAppearing()
        {


            initialized = true;
            DarkModeSwitch.IsChecked = ThemeManager.darkMode;

            if (Device.RuntimePlatform == Device.UWP)
            {
                DarkModeSwitch.IsVisible = false;
            }

            /*if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt")))
            {
                LogLabel.Text = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt"));
            }*/
        }

        protected override void OnDisappearing()
        {
            Config.Save();
        }

        private void DarkModeSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (initialized && Device.RuntimePlatform != Device.UWP)
            {
                ThemeManager.darkMode = DarkModeSwitch.IsChecked;

                MainPage.config.darkMode = DarkModeSwitch.IsChecked;
            }
        }

        private void EasterEggSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (EasterEggSwitch.IsChecked)
            {
                DisplayAlert("1 ??? :)", "Świątynią prędkości ten tor nazywają", "Szukam następnych");
                EasterEggSwitch.IsChecked = false;
            }
        }
    }
}