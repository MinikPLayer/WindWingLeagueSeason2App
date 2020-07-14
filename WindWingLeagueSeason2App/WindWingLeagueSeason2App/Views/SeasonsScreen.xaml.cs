using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Collections.Generic;
using System.Globalization;

using WindWingLeagueSeason2App.Models;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SeasonsScreen : ContentPage
    {
        public static List<Season> seasons;

        public static Season seasonSelected;

        public SeasonsScreen()
        {
            InitializeComponent();

            Title = "Sezony";

            if(seasons == null)
            {
                GetSeasons();
            }
        }

        public static async Task SelectSeason(int id)
        {
            if (id < 0 || id >= seasons.Count)
            {
                Debug.LogError("Selected season not found");
                return;
            }
            seasonSelected = seasons[id];



            MenuPage.actualMenu.UpdateItems(false);

            //await MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Leaderboards);
            MainPage.singleton.CloseAllPages();
        }

        public static async Task GetSeasons()
        {
            try
            {
                while(!MainPage.networkData.connected)
                {
                    await Task.Delay(10);
                }

                //MainPage.singleton.DisplayAlert("Getting seasons", "Tak", "OK");

                string response = await MainPage.networkData.RequestAsync("Info;SC"); // Seasons count
                int count = int.Parse(response);

                seasons = new List<Season>(count);

                for (int i = 0;i<count;i++)
                {
                    response = await MainPage.networkData.RequestAsync("Info;SD;" + (i+1).ToString());

                    string[] data = response.Split(';');

                    //seasons.Add(new Season { id = int.Parse(infos[0]), races = new List<Race>(int.Parse(infos[1])), drivers = new List<Driver>(int.Parse(infos[2])) });
                    var season = new Season(data[0]);

                    Debug.Log("[SeasonsScreen.GetSeasons] Response: " + response);
                    Debug.Log("[SeasonsScreen.GetSeasons] Data count: " + data.Length.ToString());

                    if(data.Length > 1) // Also, user registered data
                    {
                        season.userRegistered = bool.Parse(data[1]);
                        Debug.Log("User registered: " + season.userRegistered);
                    }

                    seasons.Add(season);
                }

                if(seasons.Count == 0)
                {
                    await MainPage.singleton.DisplayAlert("Błąd", "Nie znaleziono żadnego sezonu", "Wyjdź");
                    Environment.Exit(-1);
                }
                seasonSelected = seasons[0];
            }
            catch(Exception e)
            {
                Debug.Log("Error parsing seasons\n" + e.ToString());
                //MainPage.singleton.DisplayAlert("Błąd", "Błąd przetwarzania danych sezonów", "OK");
            }
        }

    }
}