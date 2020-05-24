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

            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt")))
            {
                LogLabel.Text = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt"));
            }
        }

        protected override void OnDisappearing()
        {
            Config.Save();
        }

        private void DarkModeSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (initialized)
            {
                ThemeManager.darkMode = DarkModeSwitch.IsChecked;

                MainPage.config.darkMode = DarkModeSwitch.IsChecked;
            }
        }
    }
}