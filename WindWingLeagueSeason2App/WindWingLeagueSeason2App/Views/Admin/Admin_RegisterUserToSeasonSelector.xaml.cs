using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindWingLeagueSeason2App.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindWingLeagueSeason2App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Admin_RegisterUserToSeasonSelector : ContentPage
    {
        public class UserEntry : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string name { get; set; }

            public bool empty { get; set; }

            public User user;
        }

        public ObservableCollection<UserEntry> entries { get; set; }

        public static Season season;

        public Admin_RegisterUserToSeasonSelector()
        {
            InitializeComponent();

            Title = "Wybierz użytkownika";

            entries = new ObservableCollection<UserEntry>();
        }

        protected override void OnAppearing()
        {
            BindingContext = this;

            if(season == null)
            {
                season = SeasonsScreen.seasonSelected;
            }

            UpdateUsersList();
        }

        async Task UpdateUsersList()
        {
            try
            {
                string[] datas = (await MainPage.networkData.RequestAsync("Admin;GetUsers")).Split(';');
                if (datas[0] == "OK")
                {
                    int count = -1;
                    string data = "";
                    for (int i = 0; i < datas[1].Length; i++)
                    {
                        if (datas[1][i] == '{')
                        {
                            count = int.Parse(datas[1].Substring(0, i));
                            data = datas[1].Substring(0, datas[1].Length - 1).Remove(0, i+1); // Remove 25{ and }
                            break;
                        }
                    }

                    if(count == 0)
                    {
                        DisplayAlert("Błąd", "Nie udało się przetworzyć danych użytkowników, brak klamry otwierającej", "OK");
                        MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
                        return;
                    }

                    string[] dt = data.Split(',');
                    if(dt.Length != count)
                    {
                        DisplayAlert("Błąd", "Nie udało się przetworzyć danych użytkowników, błędna suma kontrolna", "OK");
                        MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
                        return;
                    }

                    entries.Clear();
                    int added = 0;
                    for(int i = 0;i<count;i++)
                    {
                        var user = await User.GetUser(int.Parse(dt[i]));
                        bool found = false;
                        for(int j = 0;j<season.users.Count;j++)
                        {
                            if(user.id == season.users[j].user.id)
                            {
                                found = true;
                                break;
                            }
                        }
                        if(!found)
                        {
                            AddEntry(user);
                            added++;
                        }
                    }
                    if(added == 0)
                    {
                        entries.Add(new UserEntry { name = "Nie znaleziono użytkowników niezarejestrowanych do sezonu", empty = true });
                    }
                }
                else
                {
                    DisplayAlert("Błąd", "Nie udało się pobrać danych użytkowników", "OK");
                    MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
                    return;
                }
            }
            catch(Exception e)
            {
                DisplayAlert("Błąd", "Błąd przetwarzania danych użytkowników: " + e.ToString(), "OK");
                MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
                return;
            }
        }

        void AddEntry(User entry)
        {
            entries.Add(new UserEntry { name = entry.login, empty = false, user = entry });
        }

        private void EntriesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem == null)
            {
                return;
            }
            UserEntry item = e.SelectedItem as UserEntry;
            if(item.empty)
            {
                MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
                return;
            }
            Admin_RegisterUserToSeason.user = item.user;
            Admin_RegisterUserToSeason.season = season;
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_RegisterSeasonUser, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector);
        }
    }
}