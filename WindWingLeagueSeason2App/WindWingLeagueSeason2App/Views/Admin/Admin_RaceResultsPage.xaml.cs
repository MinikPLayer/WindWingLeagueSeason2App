using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindWingLeagueSeason2App.Models;
using WindWingLeagueSeason2App.Views.Admin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindWingLeagueSeason2App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Admin_RaceResultsPage : ContentPage
    {
        public class ResultsEntry : INotifyPropertyChanged
        {
            public Race.Result result;

            public int place
            {
                get
                {
                    return result.place;
                }
                set
                {
                    result.place = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("name"));
                    }
                }
            }

            string _prefix;
            public string prefix
            {
                get
                {
                    return _prefix;
                }
                set
                {
                    _prefix = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("name"));
                    }
                }
            }

            string _team;
            public string team
            {
                get
                {
                    return _team;
                }
                set
                {
                    _team = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("name"));
                    }
                }
            }

            public string rawName
            {
                get
                {
                    return _name;
                }
            }

            string _name;
            public string name
            {
                get
                {
                    string placeStr;
                    if (result?.dnf == true || place == -1)
                    {
                        placeStr = "DNF";
                    }
                    else if (result?.started == false || place == -2)
                    {
                        placeStr = "DNS";
                    }
                    else
                    {
                        placeStr = place.ToString();
                    }
                    string str = prefix + placeStr + ") " + _name;
                    if (team != null && team.Length > 0) //+ " [" + team + "]"; 
                    {
                        str += " [" + team + "]";
                    }
                    return str;
                }
                set
                {
                    _name = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("name"));
                    }
                }
            }

            TimeSpan _bestLap = TimeSpan.Zero;
            public TimeSpan bestLap
            {
                get
                {
                    return _bestLap;
                }
                set
                {
                    _bestLap = value;
                    bestLapStr = _bestLap.ToString("mm':'ss':'fff");
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("bestLap"));
                    }
                }
            }

            string _gapStr = "";
            public string gapStr {
                get
                {
                    /*if(place == -1)
                    {
                        return "DNF";
                    }
                    if(place == -2)
                    {
                        return "DNS";
                    }*/
                    if(result?.dnf == true || place == -1)
                    {
                        return "DNF";
                    }
                    if (result?.started == false)
                    {
                        if(result.dnsu)
                        {
                            return "DND";
                        }
                        if (place == -2)
                        {
                            return "DNS";
                        }
                    }
                    
                    return _gapStr;
                }
                set
                {
                    _gapStr = value;
                }
            }

            public string bestLapStr { get; set; } = "";

            TimeSpan _gap = TimeSpan.Zero;
            public TimeSpan gap
            {
                get
                {
                    return _gap;
                }
                set
                {
                    _gap = value;
                    if (_gap.Hours != 0)
                    {
                        gapStr = _gap.ToString("hh':'mm':'ss':'fff");
                    }
                    else
                    {
                        gapStr = _gap.ToString("mm':'ss':'fff");
                    }
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("gap"));
                    }
                }
            }

            bool _highlighted = false;
            public bool highlighted
            {
                get
                {
                    return _highlighted;
                }

                set
                {
                    _highlighted = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("color"));
                    }
                }
            }
            public Color color
            {
                get
                {
                    return highlighted ? (Color)Application.Current.Resources["DynamicNavigationPrimary"] : (Color)Application.Current.Resources["DynamicBackgroundColor"];
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public ResultsEntry()
            {
                MessagingCenter.Subscribe<object>(this, "UpdateTheme", (sender) =>
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("color"));
                    }
                });
            }
        }

        public ObservableCollection<ResultsEntry> entries { get; set; }
        public static Race race;

        public Admin_RaceResultsPage()
        {
            InitializeComponent();

            Title = "[ADMIN] Wyniki wyścigu";

            entries = new ObservableCollection<ResultsEntry>();
        }

        protected override void OnAppearing()
        {
            BindingContext = this;

            if(race == null)
            {
                DisplayAlert("Błąd", "Nie ustawiono wyścigu", "OK");
                MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RaceResults);
                return;
            }
            race.SortResults();

            RaceNameLabel.Text = race.track.country;

            UpdateLeaderboards();
        }

        void UpdateLeaderboards()
        {
            if(race.results.Count == 0)
            {
                CreateLeaderboards();
                return;
            }

            entries.Clear();
            for(int i = 0;i<race.results.Count;i++)
            {
                //entries.Add(new ResultsEntry { name = race.results[i].user.login });
                Debug.Log("Adding entry with place: " + race.results[i].place);
                AddEntry(race.results[i]);
            }
        }
        
        void AddEntry(Race.Result result)
        {
            TimeSpan firstTime = TimeSpan.Zero;
            if(entries.Count > 0)
            {
                firstTime = entries[0].gap;
            }

            entries.Add(new ResultsEntry { name = result.user.login, gap = (result.time - firstTime), bestLap = result.bestLap, result = result });
        }

        void CreateLeaderboards()
        {
            entries.Clear();
            race.results.Clear();
            for(int i = 0;i<SeasonsScreen.seasonSelected.users.Count;i++)
            {
                var result = new Race.Result(SeasonsScreen.seasonSelected.users[i].user, SeasonsScreen.seasonSelected.users[i].team, i + 1, TimeSpan.Zero, TimeSpan.Zero, false, true);
                race.results.Add(result);
                AddEntry(result);
            }
        }

        int entriesSelectedIndex = -1;
        private void EntriesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var entry = ((ResultsEntry)e.SelectedItem);
            for (int i = 0;i<entries.Count;i++)
            {
                if (entries[i] == entry)
                {
                    entries[i].highlighted = true;
                    entriesSelectedIndex = i;
                }
                else
                {
                    entries[i].highlighted = false;
                }
            }

        }

        private void MoveUpButton_Clicked(object sender, EventArgs e)
        {
            if(entriesSelectedIndex <= 0 || entriesSelectedIndex >= entries.Count) // <= 0 because first item should also be ignored
            {
                return;
            }

            var pom = entries[entriesSelectedIndex];
            entries[entriesSelectedIndex] = entries[entriesSelectedIndex - 1];
            entries[entriesSelectedIndex - 1] = pom;

            entries[entriesSelectedIndex].place = entriesSelectedIndex + 1;//++;
            entries[entriesSelectedIndex - 1].place = entriesSelectedIndex;//--;
            entriesSelectedIndex--;
        }

        private void MoveDownButton_Clicked(object sender, EventArgs e)
        {
            if (entriesSelectedIndex < 0 || entriesSelectedIndex >= entries.Count - 1) // >= entries.Count - 1 because last item should also be ignored
            {
                return;
            }

            var pom = entries[entriesSelectedIndex];
            entries[entriesSelectedIndex] = entries[entriesSelectedIndex + 1];
            entries[entriesSelectedIndex + 1] = pom;

            entries[entriesSelectedIndex].place = entriesSelectedIndex + 1;//--;
            entries[entriesSelectedIndex + 1].place = entriesSelectedIndex + 2; //++;
            entriesSelectedIndex++;
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {

            if (entries.Count != 0)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    try
                    {
                        entries[i].result.bestLap = TimeSpan.ParseExact(entries[i].bestLapStr, "mm':'ss':'fff", null); //entries[i].bestLap;
                        entries[i].result.started = true;
                        if (entries[i].gapStr == "DNF")
                        {
                            entries[i].result.place = -1;
                            entries[i].result.time = TimeSpan.Zero;//new TimeSpan(0, 9, 59, 59, 999);
                            entries[i].result.dnf = true;
                        }
                        else if(entries[i].gapStr == "DNS")
                        {
                            entries[i].result.place = -2;
                            entries[i].result.time = TimeSpan.Zero;//new TimeSpan(0, 9, 59, 59, 999);
                            entries[i].result.started = false;
                        }
                        else if(entries[i].gapStr == "DND")
                        {
                            entries[i].result.place = -3;
                            entries[i].result.time = TimeSpan.Zero;
                            entries[i].result.started = false;
                            entries[i].result.dnsu = true;
                        }
                        else
                        {
                            
                            try
                            {
                                entries[i].result.time = TimeSpan.ParseExact(entries[i].gapStr, "hh':'mm':'ss':'fff", null);
                            }
                            catch(System.FormatException ex)
                            {
                                Debug.Log("Trying to parse with 2nd type");
                                entries[i].result.time = TimeSpan.ParseExact(entries[i].gapStr, "mm':'ss':'fff", null);
                            }
                            if (i != 0)
                            {
                                entries[i].result.time += entries[0].result.time;
                            }
                        }
                        Debug.Log("entries[" + i.ToString() + "].result.time: " + entries[i].result.time.ToString("hh':'mm':'ss':'fff"));

                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Błąd", "Błędny format czasu, użytkownik nr " + (i + 1).ToString(), "OK");
                        return;
                    }
                }
            }

            race.SortResults();

            // Check if places are ok
            for(int i = 0;i<race.results.Count;i++)
            {
                if (race.results[i].place < 0) continue;
                if(race.results[i].place != i + 1)
                {
                    race.results[i].place = i + 1;
                }
            }
            

            BackButton_Clicked(sender, e);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RaceResults);
        }

        Entry lastFocus = null;
        private void GapEntry_Focused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;

            if (entry != lastFocus)
            {
                Debug.Log("Focused entry " + entry.Text);
                lastFocus = entry;
            }
        }

        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            CreateLeaderboards();
        }
    }
}