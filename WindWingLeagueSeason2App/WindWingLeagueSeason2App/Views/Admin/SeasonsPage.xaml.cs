﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WindWingLeagueSeason2App.Views;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using WindWingLeagueSeason2App.Models;

namespace WindWingLeagueSeason2App.Views.Admin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SeasonsPage : ContentPage
    {
        public ObservableCollection<DriverEntry> drivers { get; set; }
        public ObservableCollection<RaceEntry> races { get; set; }

        public ObservableCollection<string> tracks { get; set; }

        public SeasonsPage()
        {
            InitializeComponent();

            Title = "[ADMIN] Sezony";

            drivers = new ObservableCollection<DriverEntry>();
            races = new ObservableCollection<RaceEntry>();
            tracks = new ObservableCollection<string>();

            BindingContext = this;
        }

        void UpdateSeason()
        {
            
            if (SeasonsScreen.seasonSelected == null)
            {
                return;
            }
            IsBusy = true;

            SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id.ToString();

            drivers.Clear();
            races.Clear();
            tracks.Clear();

            for(int i = 0;i<SeasonsScreen.seasonSelected.races.Count;i++)
            {
                races.Add(new RaceEntry{ race = SeasonsScreen.seasonSelected.races[i]});
            }

            for(int i = 0;i<SeasonsScreen.seasonSelected.users.Count;i++)
            {
                if (SeasonsScreen.seasonSelected.users[i].user == null)
                {
                    Debug.LogError("USER IS NULL");
                    DisplayAlert("Błąd", "Informacje o użytkowniku z ID " + SeasonsScreen.seasonSelected.users[i].id + " nie zostały poprawnie pobrane", "OK");
                }
                else
                {
                    //drivers.Add(new Models.DriverEntry { name = SeasonsScreen.seasonSelected.users[i].user.login });
                    drivers.Add(new DriverEntry { user = SeasonsScreen.seasonSelected.users[i].user, seasonUser = SeasonsScreen.seasonSelected.users[i] });
                }
            }

            for(int i = 0;i<Track.tracks.Length;i++)
            {
                tracks.Add(Track.tracks[i].country);
            }
            DisableButtons();
            IsBusy = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateSeason();

            for(int i = 0;i<SeasonsScreen.seasons.Count;i++)
            {
                if(SeasonsScreen.seasonSelected == SeasonsScreen.seasons[i])
                {
                    DisableButtons(i);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            SeasonsScreen.GetSeasons();
        }

        async void AddSeason()
        {
            //string seasonStr = "Season{track{c(Francja)},finished{True},races{race{track{id(0)},date{01.06.2020 20:00:00}},race{track{id(1)},date{02.06.2020 20:00:00}}}}";
            var season = new Season(SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1, 0, 0, Track.GetTrack(5), new List<Race> {
                /*new Race(1, new DateTime(2020, 06, 01, 20, 00, 00)),
                new Race(2, new DateTime(2020, 06, 02, 20, 00, 00)),
                new Race(3, new DateTime(2020, 06, 03, 20, 00, 00)),
                new Race(4, new DateTime(2020, 06, 04, 20, 00, 00)),
                new Race(5, new DateTime(2020, 06, 05, 20, 00, 00)),
                new Race(6, new DateTime(2020, 06, 06, 20, 00, 00)),
                new Race(7, new DateTime(2020, 06, 07, 20, 00, 00)),
                new Race(8, new DateTime(2020, 06, 08, 20, 00, 00)),
                new Race(9, new DateTime(2020, 06, 09, 20, 00, 00)),
                new Race(10, new DateTime(2020, 06, 10, 20, 00, 00))*/
            }, new RegistrationData(true, new DateTime(2020,06,06,23,59,59)));

            string seasonStr = season.Serialize();

            //string response = await MainPage.networkData.RequestAsync("Admin;Season;Add;" + (SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1).ToString());
            //string response = await MainPage.networkData.RequestAsync("Admin;Season;Add;" + (SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1).ToString() + ";" + seasonStr);
            string response = await MainPage.networkData.RequestAsync("Admin;Season;Add;" + seasonStr);
            if (response == "OK")
            {
                await MenuPage.actualMenu.UpdateSeasons(true, false);
                //MenuPage.actualMenu._SeasonPicker.IsEnabled = false;
                //MenuPage.actualMenu._SeasonPicker.SelectedIndex = SeasonsScreen.seasons.Count - 1;
                //MenuPage.actualMenu._SeasonPicker.IsEnabled = true;
                SeasonsScreen.seasonSelected = SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1];

                SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id.ToString();

                UpdateSeason();
            }
            else
            {
                string[] data = response.Split(';');
                if(data.Length > 1)
                {
                    DisplayAlert("Błąd", data[1], "OK");
                }
                else
                {
                    DisplayAlert("Błąd", data[0], "OK");
                }
                
            }

            
        }

        private void AddSeasonButton_Clicked(object sender, EventArgs e)
        {
            AddSeason();
        }

        async void Save()
        {
            var season = SeasonsScreen.seasonSelected.Serialize();
            string response = await MainPage.networkData.RequestAsync("Admin;Season;Modify;" + SeasonsScreen.seasonSelected.id.ToString() + ";" + season);
            if(response != "OK")
            {
                string[] data = response.Split(';');
                if(data.Length > 1)
                {
                    DisplayAlert("Błąd", "Błąd zapisywania sezonu na server: " + data[1], "OK");
                }
                else
                {
                    DisplayAlert("Błąd", "Błąd zapisywania sezonu na server: " + data[0], "OK");
                }
            }
            else
            {
                DisplayAlert("Zapisano", "Zapisywanie sezonu powiodło się", "OK");
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Save();
        }

        void DisableButtons(int index = -1)
        {
            if(index == -1)
            {
                for(int i = 0;i<SeasonsScreen.seasons.Count;i++)
                {
                    if(SeasonsScreen.seasons[i] == SeasonsScreen.seasonSelected)
                    {
                        index = i;
                        break;
                    }
                }
            }
            if(index == -1)
            {
                Debug.LogError("Cannot find active season");
                return;
            }

            PrevSeasonButton.IsEnabled = true;
            NextSeasonButton.IsEnabled = true;

            if (index == SeasonsScreen.seasons.Count - 1)
            {
                NextSeasonButton.IsEnabled = false;
            }
            if(index == 0)
            {
                PrevSeasonButton.IsEnabled = false;
            }
        }

        private void NextSeasonButton_Clicked(object sender, EventArgs e)
        {
            for(int i = 0;i<SeasonsScreen.seasons.Count - 1;i++)
            {
                if(SeasonsScreen.seasons[i] == SeasonsScreen.seasonSelected)
                {
                    SeasonsScreen.seasonSelected = SeasonsScreen.seasons[++i];

                    DisableButtons(i);
                    break;
                }
            }

            UpdateSeason();
        }

        private void PrevSeasonButton_Clicked(object sender, EventArgs e)
        {
            for (int i = 1; i < SeasonsScreen.seasons.Count; i++)
            {
                if (SeasonsScreen.seasons[i] == SeasonsScreen.seasonSelected)
                {
                    SeasonsScreen.seasonSelected = SeasonsScreen.seasons[--i];

                    DisableButtons(i);
                    break;
                }
            }

            UpdateSeason();
        }

        private void RacesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            RaceEntry entry = (RaceEntry)e.SelectedItem;
            for(int i = 0;i<races.Count;i++)
            {
                races[i].highlighted = false;
            }

            entry.highlighted = true;

            RaceDatePicker.Date = entry.date;
            RaceTrackPicker.SelectedItem = entry.race.track.country;
        }

        private void DriversListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DriverEntry entry = (DriverEntry)e.SelectedItem;
            for (int i = 0; i < drivers.Count; i++)
            {
                drivers[i].highlighted = false;
            }

            entry.highlighted = true;
        }

        private void DeleteRaceButton_Clicked(object sender, EventArgs e)
        {
            if(RacesListView.SelectedItem == null)
            {
                return;
            }

            RaceEntry entry = (RaceEntry)RacesListView.SelectedItem;
            SeasonsScreen.seasonSelected.races.Remove(entry.race);

            races.Remove(entry);
        }

        private void DeleteDriverButton_Clicked(object sender, EventArgs e)
        {
            if (DriversListView.SelectedItem == null)
            {
                return;
            }

            DriverEntry entry = (DriverEntry)DriversListView.SelectedItem;
            SeasonsScreen.seasonSelected.users.Remove(entry.seasonUser);

            drivers.Remove(entry);
        }

        private void AddRaceButton_Clicked(object sender, EventArgs e)
        {
            if(RaceTrackPicker.SelectedItem == null)
            {
                return;
            }

            var entry = new RaceEntry { race = new Race(RaceTrackPicker.SelectedIndex, RaceDatePicker.Date.AddHours(20)) };
            races.Add(entry);
            SeasonsScreen.seasonSelected.races.Add(entry.race);

            RacesListView.SelectedItem = entry;
        }

        private void ChangeRaceButton_Clicked(object sender, EventArgs e)
        {
            if(RacesListView.SelectedItem == null)
            {
                return;
            }

            RaceEntry entry = (RaceEntry)RacesListView.SelectedItem;

            //SeasonsScreen.seasonSelected.races.Remove(entry.race);
            //entry.race = new Race(RaceTrackPicker.SelectedIndex, RaceDatePicker.Date.AddHours(20));
            //SeasonsScreen.seasonSelected.races.Add(entry.race);
            bool modified = false;
            for(int i = 0;i<SeasonsScreen.seasonSelected.races.Count;i++)
            {
                if(SeasonsScreen.seasonSelected.races[i] == entry.race)
                {
                    entry.race = SeasonsScreen.seasonSelected.races[i] = new Race(RaceTrackPicker.SelectedIndex, RaceDatePicker.Date.AddHours(20));
                    modified = true;
                    break;
                }
            }

            if(!modified)
            {
                DisplayAlert("Desynchronizacja danych", "Uruchom ponownie tą kartę", "OK");
            }
        }

        async void DeleteSeason()
        {
            bool result = await DisplayAlert("Na pewno?", "Czy na pewno chcesz usunąć sezon nr " + SeasonsScreen.seasonSelected.id.ToString() + "?", "Tak", "Nie");
            if(result)
            {
                string response = await MainPage.networkData.RequestAsync("Admin;Season;Remove;" + SeasonsScreen.seasonSelected.id.ToString());
                if(response == "OK")
                {
                    await MenuPage.actualMenu.UpdateSeasons(true, false);

                    DisplayAlert("Usunięto", "Sezon został usunięty pomyślnie", "OK");
                }
                else
                {
                    string[] data = response.Split(';');
                    if(data.Length > 1)
                    {
                        DisplayAlert("Błąd", "Błąd usuwania sezonu: " + data[1], "OK");
                    }
                    else
                    {
                        DisplayAlert("Błąd", "Błąd usuwania sezonu: " + data[0], "OK");
                    }
                }
            }
        }

        private void DeleteSeasonButton_Clicked(object sender, EventArgs e)
        {
            DeleteSeason();
        }
    }
}