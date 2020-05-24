using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Collections.Generic;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SeasonsScreen : ContentPage
    {
        

        public class Track
        {
            public int id;
            public string name;
            public string country;
            public string city;
            public int length;
            public TimeSpan record;

            public Track(int id, string name, string country, string city, int length)
            {
                this.id = id;
                this.name = name;
                this.country = country;
                this.city = city;
                this.length = length;

                this.record = new TimeSpan(0, 0, 0, 0, 0);
            }

            public Track(int id, string name, string country, string city, int length, TimeSpan record)
            {
                this.id = id;
                this.name = name;
                this.country = country;
                this.city = city;
                this.length = length;

                this.record = record;
            }
        }

        public class Team
        {
            public int id;
            public string name;
            public string shortName;
            public string iconPath;

            public Team(int id, string name, string shortName, string iconPath)
            {
                this.id = id;
                this.name = name;
                this.shortName = shortName;
                this.iconPath = iconPath;
            }
        }

        public class Driver
        {
            public int id;
            public string login;
            public string steam;
            public TimeSpan lapDry;
            public TimeSpan lapWet;
            public int priority;

            public Driver()
            {

            }

            public Driver(int id, string login, string steam)
            {
                FillVariables(id, login, steam);
            }

            public void FillVariables(int id, string login, string steam)
            {
                this.id = id;
                this.login = login;
                this.steam = steam;
            }

        }

        public class Race
        {
            public string country;
        }

        public class Season
        {
            public int id;
            public List<Race> races = new List<Race>();
            public List<Driver> drivers = new List<Driver>();
        }

        // To do, implement data downloading from server
        public Track[] tracks = new Track[] {
                new Track(0, "Albert Park Circuit", "Australia", "Melbourne", 5303  ),
                new Track(1, "Bahrain International Circuit", "Bahrain", "Sakhir", 5412),
                new Track(2, "Shanghai International Circuit", "Chiny", "Shanghai", 5451 ),
                new Track(3, "Baku City Circuit", "Azerbejdzan", "Baku", 6003),
                new Track(4, "Circuit de Barcelona-Catalunya", "Hiszpania", "Montmelo", 4655 ),
                new Track(5, "Circuit de Monaco", "Monako", "Monako", 3337),
                new Track(6, "Circuit Gilles Villeneuve", "Kanada", "Montreal", 4361),
                new Track(7, "Circuit Paul Ricard", "Francja", "Le Castellet", 5842),
                new Track(8, "Red Bull Ring", "Austria", "Spielberg", 4318),
                new Track(9, "Silverstone", "Wielka Brytania", "Silverstone", 5891),
                new Track(10, "Hockenheimring", "Niemcy", "Hockenheim", 4574),
                new Track(11, "Hungaroring", "Wegry", "Mogyorod", 4381),
                new Track(12, "Circuit de Spa-Francorchamps", "Belgia", "Stavelot", 7004),
                new Track(13, "Autodromo Nationale Monza", "Wlochy", "Monza", 5793),
                new Track(14, "Marina Bay Street Circuit", "Singapur", "Singapur", 5063),
                new Track(15, "Sochi Autodrom", "Rosja", "Sochi", 5848),
                new Track(16, "Suzuka Circuit", "Japonia", "Suzuka", 5807),
                new Track(17, "Autódromo Hermanos Rodríguez", "Meksyk", "Mexico City", 4304),
                new Track(18, "Circuit of the Americas", "USA", "Austin", 5513),
                new Track(19, "Autódromo José Carlos Pace", "Brazylia", "Sao Paulo", 4309),
                new Track(20, "Yas Marina Circuit", "Zjedoczone Emiraty Arabskie", "Abu Dhabi", 5554)
        };

        public Team[] teams = new Team[] {
            new Team(0, "Mercedes", "MER", ""),
            new Team(1, "Ferrari", "FER", ""),
            new Team(2, "Red Bull", "RDB", ""),
            new Team(3, "Renault", "REN", ""),
            new Team(4, "Haas", "HAS", ""),
            new Team(5, "McLaren", "MCL", ""),
            new Team(6, "Racing Point", "RPT", ""),
            new Team(7, "Alfa Romeo", "ARM", ""),
            new Team(8, "Toro Rosso", "TRS", ""),
            new Team(9, "Williams", "WIL", ""),
            new Team(10, "Other", "OTH", "")
        };

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
            if (id < 0 || id >= seasons.Count) return;
            seasonSelected = seasons[id];

            await MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Leaderboards);
            MainPage.singleton.CloseAllPages();
        }

        // TO DO
        public static async Task GetSeasons()
        {
            try
            {
                string response = await MainPage.networkData.RequestAsync("Info;SC"); // Seasons count
                int count = int.Parse(response);

                seasons = new List<Season>(count);

                for (int i = 0;i<count;i++)
                {
                    response = await MainPage.networkData.RequestAsync("Info;SD;" + (i+1).ToString());

                    if(response.StartsWith("("))
                    {
                        response = response.Substring(0, response.Length - 1).Remove(0, 1);
                    }
                    else
                    {
                        Debug.Log("[SeasonsScreen.GetSeasons] Bad data format, not starting with (: "  + response);
                        return;
                    }

                    string[] infos = response.Split(',');

                    if(infos.Length < 3)
                    {
                        Debug.Log("[SeasonsScreen.GetSeasons] Bad data format, not enough information: " + response);
                        return;
                    }

                    seasons.Add(new Season { id = int.Parse(infos[0]), races = new List<Race>(int.Parse(infos[1])), drivers = new List<Driver>(int.Parse(infos[2])) });

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