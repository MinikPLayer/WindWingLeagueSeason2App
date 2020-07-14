using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            string vStr = (App.appVersion / 1000).ToString() + "." + ((App.appVersion / 100) % 10).ToString() + "." + (App.appVersion % 100).ToString();

            VersionText.Text = vStr;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            DisplayAlert("?? 3 ? :)", "Złącz toru nazwę i rok tego wyścigu", "Już prawie ;)");
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {   
            DisplayAlert("??? 4 :)", "Do Minika je wyślij a dostaniesz nagród bez liku", "Już wiesz co robić ;)");
        }
    }
}