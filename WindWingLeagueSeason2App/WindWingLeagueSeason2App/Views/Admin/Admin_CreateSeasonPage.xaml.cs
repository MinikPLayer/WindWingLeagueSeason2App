using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WindWingLeagueSeason2App.Views;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using WindWingLeagueSeason2App.Models;

using Xamarin.Essentials;

namespace WindWingLeagueSeason2App.Views.Admin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class Admin_CreateUserPage : ContentPage
    {
        Season season;

        public ObservableCollection<string> tracks { get; set; }

        public Admin_CreateUserPage()
        {
            InitializeComponent();

            Title = "[ADMIN] Utwórz sezon";

            tracks = new ObservableCollection<string>();

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(season == null)
            {
                season = new Season(SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1, 0, 0, Track.tracks[0], null, null);
            }

            tracks.Clear();
            for(int i = 0;i<Track.tracks.Length;i++)
            {
                tracks.Add(Track.tracks[i].country);
            }

            SeasonNameText.Text = "Sezon " + season.id.ToString();

        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_CreateSeason);
        }
        
       
        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            int gameVersion;

            if(RegisterTrackPicker.SelectedItem == null)
            {
                DisplayAlert("Błąd", "Musisz wybrać tor rejestracyjny", "OK");
                return;
            }
            if(!int.TryParse(GameVersionText.Text, out gameVersion))
            {
                DisplayAlert("Błąd", "Błędny format wersji gry ( wpisz rok, np 2019 lub 2020 )", "OK");
                return;
            }

            season.gameVersion = gameVersion;
            season.registrationData = new RegistrationData(true, EndTimePicker.Date);
            season.registrationTrack = Track.GetTrack((string)RegisterTrackPicker.SelectedItem);

            SaveSeason();
        }

        async void SaveSeason()
        {
            string seasonStr = season.Serialize();

            string response = await MainPage.networkData.RequestAsync("Admin;Season;Add;" + seasonStr);
            if (response == "OK")
            {
                DisplayAlert("Utworzono", "Pomyslnie utworzono sezon", "OK");
                await SeasonsScreen.GetSeasons();

                BackButton_Clicked(this, null);
            }
            else
            {
                string[] data = response.Split(';');
                if (data.Length > 1)
                {
                    DisplayAlert("Błąd", data[1], "OK");
                }
                else
                {
                    DisplayAlert("Błąd", data[0], "OK");
                }

            }


        }
    }
}