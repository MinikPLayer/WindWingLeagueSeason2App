using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindWingLeagueSeason2App.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindWingLeagueSeason2App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeamLeaderboardsPage : ContentPage
    {
        public ObservableCollection<LeaderboardsTeamEntry> entries { get; set; }

        bool prefixesAdded = false;

        Team GetTeam(string name)
        {
            return Team.GetTeamShort(name);
        }

        public TeamLeaderboardsPage()
        {
            InitializeComponent();

            Title = "Ranking drużyn";

            entries = new ObservableCollection<LeaderboardsTeamEntry>();            
        }

        protected override void OnAppearing()
        {
            BindingContext = this;

            entries.Clear();
            UpdateLeaderboards();
        }

        async Task UpdateLeaderboards()
        {
            //string data = await MainPage.networkData.RequestAsync("Leaderboards");
            while(SeasonsScreen.seasonSelected == null)
            {
                await DisplayAlert("OK", "Season Selected is null, Updating", "OK");
                //await MenuPage.actualMenu.UpdateSeasons(true);
                await Task.Delay(100);
            }

            string data = await MainPage.networkData.RequestAsync("TeamLeaderboards;" + SeasonsScreen.seasonSelected.id.ToString());

            Debug.Log("Leaderboards response: " + data);
            
            
            ParseData(data);

        }

        void AddEntry(LeaderboardsTeamEntry entry)
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
                    LeaderboardsTeamEntry pom = entries[maxPos];
                    entries[maxPos] = entries[i];
                    entries[i] = pom;
                }
            }
        }    

        async void ParseData(string data)
        {
            try
            {
                if(data == "NS")
                {
                    AddEntry(new LeaderboardsTeamEntry { name = "Brak danych" });
                    return;
                }

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
                    if(App.debug)
                    {
                        DisplayAlert("Ranking", "Brak danych w pakiecie", "OK");
                    }
                    return;
                }

                //entries.Clear();

                for (int i = 0;i<number;i++)
                {
                    datas[i] = datas[i].Substring(1, datas[i].Length - 2);
                    string[] infos = datas[i].Split(',');
                    if(infos.Length < 2)
                    {
                        MainPage.log += "[ERROR] - {Parse Leaderboards Data}  Not enough info in data";
                        if (App.debug)
                        {
                            DisplayAlert("Ranking", "Brak wystarczającej ilości danych w pakiecie", "OK");
                        }
                        return;
                    }

                    int id = int.Parse(infos[0]);
                    Team t = Team.GetTeam(id);
                    if(t == null)
                    {
                        DisplayAlert("Błąd", "Błąd pobierania danych o tabeli - użytkownik nie istnieje", "OK");
                        return;
                    }
                    if (t == Team.other) continue;
                    AddEntry(new LeaderboardsTeamEntry { name = t.name, score = int.Parse(infos[1]), team = t });
                }

#if DEBUG
//DisplayAlert("Pobrano", "Entries count: " + entries.Count.ToString(), "OK");
#endif
            }
            catch(Exception e)
            {
                MainPage.log += "[ERROR] " + e.ToString();

                DisplayAlert("Blad pakietu", "Nie udalo sie przetworzyc pakietu rankingu, blad: \n " + e.ToString(), "OK");
            }

        }

        async void MoveToTeamInfo(LeaderboardsTeamEntry entry)
        {
            //Season.SeasonUser u = await SeasonsScreen.seasonSelected.GetUser(entry.user.id);
            /*for(int i = 0;i<SeasonsScreen.seasonSelected.users.Count;i++)
            {
                if(SeasonsScreen.seasonSelected.users[i].user.id == entry.user.id)
                {
                    u = SeasonsScreen.seasonSelected.users[i];
                    break;
                }
            }*/
            /*if (u == null)
            {
                DisplayAlert("Nieoczekiwany błąd", "Nie znaleziono użytkownika w sezonie", "OK");
                return;
            }*/

            //DriverInfoPage.user = u;
            //DriverInfoPage.entry = entry;

            //MainPage.singleton.NavigateFromMenu((int)MenuItemType.DriverInfo);
        }

        private void EntriesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            LeaderboardsTeamEntry entry = (LeaderboardsTeamEntry)e.SelectedItem;
            if (entry == null) return;

            MoveToTeamInfo(entry);
        }
    }
}