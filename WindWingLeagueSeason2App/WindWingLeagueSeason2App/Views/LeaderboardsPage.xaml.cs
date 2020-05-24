using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindWingLeagueSeason2App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderboardsPage : ContentPage
    {
        public ObservableCollection<Models.LeaderboardsEntry> entries { get; set; }

        bool prefixesAdded = false;

        class Team
        {
            public string name;
            public string shortName;

            public Team(string name, string shortName)
            {
                this.name = name;
                this.shortName = shortName;
            }
        }

        Team[] teams = new Team[] { new Team("Mercedes", "MER"), new Team("Ferrari", "FRI"), new Team("Red Bull", "RDB"), new Team("McLaren", "MCL"), new Team("Racing Point", "RPT"), new Team("Renault", "RNL"), new Team("Haas", "HAS"), new Team("Williams", "WIL"), new Team("Alfa Romeo", "ARO"), new Team("Toro Rosso", "TRS"), new Team("Other", "OTH") }; 

        Team GetTeam(string name)
        {
            for(int i = 0;i<teams.Length;i++)
            {
                if(teams[i].shortName == name)
                {
                    return teams[i];
                }
            }
            return null;
        }

        public LeaderboardsPage()
        {
            InitializeComponent();

            Title = "Ranking";

            entries = new ObservableCollection<Models.LeaderboardsEntry>();

            BindingContext = this;

            //AddEntry(new Models.LeaderboardsEntry { name = "Test", score = 30 });
            //AddEntry(new Models.LeaderboardsEntry { name = "Test2", score = 60 });

            UpdateLeaderboards();
        }

        async Task UpdateLeaderboards()
        {
            //string data = await MainPage.networkData.RequestAsync("Leaderboards");
            string data = await MainPage.networkData.RequestAsync("Leaderboards");

            ParseData(data);

        }

        void AddEntry(Models.LeaderboardsEntry entry)
        {
            IsBusy = true;
            RemovePrefixes();
            entries.Add(entry);
            SortEntries();
            AddPrefixes();
            HighlightCurrentUser();
            IsBusy = false;
        }

        void HighlightCurrentUser()
        {
            for(int i = 0;i<entries.Count;i++)
            {
                if(entries[i].rawName == LoginScreen.username)
                {
                    entries[i].highlighted = true;
                }
            }
        }

        void RemovePrefixes()
        {
            if(!prefixesAdded)
            {
                return;
            }

            for(int i = 0;i<entries.Count;i++)
            {
                for(int j = 0;j<entries[i].name.Length;j++)
                {
                    if(entries[i].name[j] == ')')
                    {
                        //entries[i].name = entries[i].name.Remove(0, j + 2); // "1) "
                        entries[i].prefix = "";
                        break;
                    }
                }
            }

            prefixesAdded = false;
        }

        void AddPrefixes()
        {
            for(int i = 0;i<entries.Count;i++)
            {
                //entries[i].name = entries[i].name.Insert(0, (i + 1).ToString() + ") ");
                entries[i].prefix = (i + 1).ToString() + ") ";
            }

            prefixesAdded = true;
        }

        void SortEntries()
        {
            for (int i = 0; i < entries.Count - 1; i++)
            {
                int max = int.MinValue;
                int maxPos = -1;
                for (int j = i; j < entries.Count; j++)
                {
                    if(entries[j].score > max)
                    {
                        maxPos = j;
                        max = entries[j].score;
                    }
                }

                if(maxPos != i && maxPos != -1)
                {
                    Models.LeaderboardsEntry pom = entries[maxPos];
                    entries[maxPos] = entries[i];
                    entries[i] = pom;
                }
            }
        }    

        void ParseData(string data)
        {
            try
            {
                string numStr = "";
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == '{')
                    {
                        data = data.Remove(data.Length - 1, 1).Remove(0, i + 1);                   
                        break;
                    }
                    else
                    {
                        numStr += data[i];
                    }
                }

                int number = int.Parse(numStr);

                string[] datas = data.Split(';');

                if(datas.Length == 0)
                {
                    MainPage.log += "[ERROR] - {Parse Leaderboards Data}  No datas found";
                    return;
                }

                for(int i = 0;i<number;i++)
                {
                    datas[i] = datas[i].Substring(1, datas[i].Length - 2);
                    string[] infos = datas[i].Split(',');
                    if(infos.Length < 3)
                    {
                        MainPage.log += "[ERROR] - {Parse Leaderboards Data}  Not enough info in data";
                        return;
                    }

                    //AddEntry(new Models.LeaderboardsEntry(infos[0], int.Parse(infos[1])));
                    AddEntry(new Models.LeaderboardsEntry { name = infos[0], score = int.Parse(infos[1]), team = GetTeam(infos[2]).name });
                }
            }
            catch(Exception e)
            {
                MainPage.log += "[ERROR] " + e.ToString();
                LogLabel.Text += e.ToString();

                DisplayAlert("Blad pakietu", "Nie udalo sie przetworzyc pakietu rankingu, blad: \n " + e.ToString(), "OK");
            }

        }

        private void EntriesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            AddEntry(new Models.LeaderboardsEntry { name = "Entry " + entries.Count.ToString(), score = entries.Count });
        }
    }
}