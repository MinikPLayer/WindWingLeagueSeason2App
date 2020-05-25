using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WindWingLeagueSeason2App.Views;

namespace WindWingLeagueSeason2App.Views.Admin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SeasonsPage : ContentPage
    {
        public SeasonsPage()
        {
            InitializeComponent();

            Title = "[ADMIN] Sezony";
        }

        void UpdateSeason()
        {
            if (SeasonsScreen.seasonSelected != null)
            {
                SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id.ToString();
            }
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


        
        async void AddSeason()
        {
            //string seasonStr = "Season{track{c(Francja)},finished{True},races{race{track{id(0)},date{01.06.2020 20:00:00}},race{track{id(1)},date{02.06.2020 20:00:00}}}}";
            var season = new Models.Season(SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1, 10, false, Models.Track.GetTrack(5), new System.Collections.Generic.List<Models.Race> {
                new Models.Race(1, new DateTime(2020, 06, 01, 20, 00, 00)),
                new Models.Race(2, new DateTime(2020, 06, 02, 20, 00, 00)),
                new Models.Race(3, new DateTime(2020, 06, 03, 20, 00, 00)),
                new Models.Race(4, new DateTime(2020, 06, 04, 20, 00, 00)),
                new Models.Race(5, new DateTime(2020, 06, 05, 20, 00, 00)),
                new Models.Race(6, new DateTime(2020, 06, 06, 20, 00, 00)),
                new Models.Race(7, new DateTime(2020, 06, 07, 20, 00, 00)),
                new Models.Race(8, new DateTime(2020, 06, 08, 20, 00, 00)),
                new Models.Race(9, new DateTime(2020, 06, 09, 20, 00, 00)),
                new Models.Race(10, new DateTime(2020, 06, 10, 20, 00, 00))
            }, new Models.RegistrationData(true, new DateTime(2020,06,01,20,00,00)));

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

        private void SaveButton_Clicked(object sender, EventArgs e)
        {

        }

        void DisableButtons(int index)
        {
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
    }
}