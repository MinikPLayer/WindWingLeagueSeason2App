using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WindWingLeagueSeason2App.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace WindWingLeagueSeason2App.Views.Admin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SeasonSavePage : ContentPage
    {
        public bool pageInFocus = false;

        public SeasonSavePage()
        {
            InitializeComponent();

            Title = "Login";
        }

        async Task AnimateLoginText()
        {
            int counter = 0;
            while(pageInFocus)
            {
                string text = "Zapisywanie sezonu";
                for(int i = 0;i<counter;i++)
                {
                    text += ".";
                }
                LoginText.Text = text;
                counter = (counter + 1) % 4;
                await Task.Delay(500);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pageInFocus = true;
            AnimateLoginText();
            SaveSeason();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pageInFocus = false;
        }

        async Task SaveSeason()
        {


            SeasonsScreen.seasonSelected.SortRaces();

            // TODO: Add real team changing
            Season s = SeasonsScreen.seasonSelected;
            for(int i = 0;i<s.races.Count;i++)
            {
                for(int j = 0;j<s.races[i].results.Count;j++)
                {
                    s.races[i].results[j].team = (await s.GetUser(s.races[i].results[j].user.id)).team;
                }
            }
            // END TODO

            var season = SeasonsScreen.seasonSelected.Serialize();

            int ogTimeout = MainPage.networkData.timeout;
            MainPage.networkData.timeout = 20000;

            string response = await MainPage.networkData.RequestAsync("Admin;Season;Modify;" + SeasonsScreen.seasonSelected.id.ToString() + ";" + season);
            if (response != "OK")
            {
                string[] data = response.Split(';');
                if (data.Length > 1)
                {
                    await DisplayAlert("Błąd", "Błąd zapisywania sezonu na server: " + data[1], "OK");
                }
                else
                {
                    await DisplayAlert("Błąd", "Błąd zapisywania sezonu na server: " + data[0], "OK");
                }
            }
            else
            {
                await DisplayAlert("Zapisano", "Zapisywanie sezonu powiodło się", "OK");
            }
            MainPage.networkData.timeout = ogTimeout;


            MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_SaveSeason);
        }

        
    }
}