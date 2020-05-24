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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(SeasonsScreen.seasonSelected != null)
            {
                SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id.ToString();
            }
        }


        
        async void AddSeason()
        {
            string seasonStr = "Season{track{c(Francja)},finished{True},races{race{track{id(0)},date{01.06.2020 20:00:00}},race{track{id(1)},date{02.06.2020 20:00:00}}}}";
            //string response = await MainPage.networkData.RequestAsync("Admin;Season;Add;" + (SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1).ToString());
            string response = await MainPage.networkData.RequestAsync("Admin;Season;Add;" + (SeasonsScreen.seasons[SeasonsScreen.seasons.Count - 1].id + 1).ToString() + ";" + seasonStr);
            if (response == "OK")
            {
                await MenuPage.actualMenu.UpdateSeasons(true);
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
    }
}