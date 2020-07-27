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
using SkiaSharp;
using Microcharts;
using Microcharts.Forms;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    public partial class DriverInfoPage : ContentPage
    {
        public static Season.SeasonUser user;
        public static LeaderboardsEntry entry;


        public DriverInfoPage()
        {
            InitializeComponent();

            Title = "Kierowca";

            BindingContext = this;

            
        }

        int posSum = 0;
        int resultsCount = 0;
        void CreatePlot()
        {
            int max = 20;

            posSum = 0;
            resultsCount = 0;

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            for(int i = 0;i<SeasonsScreen.seasonSelected.races.Count;i++)
            {
                Race r = SeasonsScreen.seasonSelected.races[i];
                for (int j = 0;j<r.results.Count;j++)
                {
                    if (r.results[j].user.id == user.user.id)
                    {
                        var entry = new Microcharts.Entry(r.results[j].dnf ? 0 : r.results[j].started ? max - r.results[j].place + 1 : 0)
                        {
                            Label = r.track.country,
                            ValueLabel = r.results[j].dnf ? "DNF" : r.results[j].started ? r.results[j].place.ToString() : "DNS",
                            Color = (r.results[j].started == false) ? SKColor.Parse("#DCDCDC")
                            : (r.results[j].dnf == true) ? SKColor.Parse("#D3D3D3")
                            : (r.results[j].place == 1) ? SKColor.Parse("#D4AF37") 
                            : (r.results[j].place == 2) ? SKColor.Parse("#A9A9A9") 
                            : (r.results[j].place == 3) ? SKColor.Parse("#A97142") 
                            : SKColor.Parse("#101010")
                        };

                        Debug.Log(i.ToString() + ") " + entry.Color.ToString());
                        entries.Add(entry);

                        posSum += r.results[j].dnf ? 20 : r.results[j].started ? r.results[j].place : 20;
                        resultsCount++;
                        break;
                    }
                }
            }

            var chart = new LineChart() { Entries = entries, MinValue = 0, MaxValue = max + 1, LineMode = LineMode.Straight, PointMode = PointMode.Circle };

            chartView.Chart = chart;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (user == null)
            {
                DisplayAlert("Błąd", "Nie wybrano użytkownika", "OK");
                MainPage.singleton.NavigateFromMenu((int)MenuItemType.Leaderboards, (int)MenuItemType.DriverInfo);
                return;
            }
            if(entry == null)
            {
                DisplayAlert("Błąd", "Nie wybrano wpisu w tabeli", "OK");
                MainPage.singleton.NavigateFromMenu((int)MenuItemType.Leaderboards, (int)MenuItemType.DriverInfo);
                return;
            }

            SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id.ToString();
            UsernameText.Text = user.user.login;

            TeamText.Text = user.team.GetName();
            PositionText.Text = entry.prefix.Replace(")", "");
            int points = entry.score;
            PointsText.Text = points.ToString();
            
            
            //for(int i = 0;i<LeaderboardsPage.)

            CreatePlot();

            AvergePointsText.Text = ((float)points / resultsCount).ToString("0.0");
            AvergePositionText.Text = ((float)posSum / resultsCount).ToString("0.0");

        }

        async void OpenLink(string link)
        {
            bool result = await DisplayAlert("Link", "Czy na pewno chcesz otworzyć link \"" + link + "\"?", "OK", "Anuluj");
            if (!result) return;

            try
            {
                var uri = new Uri(link);
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception e)
            {
                DisplayAlert("Błąd", "Nie udało się otworzyć linku:\"" + link + "\"", "OK");
            }
        }

        bool ParseTime(string str, out TimeSpan time)
        {
            try
            {
                char splitChar = ':';
                if (str.Contains("."))
                {
                    splitChar = '.';
                }

                string[] data = str.Split(splitChar);
                if (data.Length != 3)
                {
                    Debug.LogError("Error parsing time, bad data count");
                    return false;
                }

                while (data[2].Length < 3)
                {
                    data[2] += "0";
                }

                Debug.Log("Minutes: " + data[0] + ", seconds: " + data[1] + ", miliseconds: " + data[2]);

                time = new TimeSpan(0, 0, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]));
                return true;
            }
            catch (FormatException e)
            {
                Debug.LogError("[RegisterToSeasonPage.ParseTime] Error parsing time: error parsing numbers");
                return false;
            }
            catch (Exception e)
            {
                Debug.Exception(e, "[RegisterToSeasonPage.ParseTime]");
                return false;
            }
        }

        private void SteamLinkButton_Clicked(object sender, EventArgs e)
        {
            string link = user.user.steam;
            if (link.StartsWith("steam"))
            {
                link = "https://" + link;
            }
            Clipboard.SetTextAsync(link);
            OpenLink(link);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            user = null;
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.Leaderboards, (int)MenuItemType.DriverInfo);
        }
    }
}