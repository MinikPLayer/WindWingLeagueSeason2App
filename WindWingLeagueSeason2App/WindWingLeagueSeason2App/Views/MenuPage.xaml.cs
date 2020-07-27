using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using WindWingLeagueSeason2App.Models;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        public static MenuPage actualMenu;

        public Picker _SeasonPicker
        {
            get { return SeasonPicker; }
        }

        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            if (actualMenu == null)
            {
                UpdateItems();
            }

            actualMenu = this;
        }

        public async void UpdateItems(bool updateSeasons = true)
        {
            BindingContext = this;

            if (updateSeasons)
            {
                await UpdateSeasons(true);
            }

            if (LoginScreen.loggedIn)
            {
                menuItems = new List<HomeMenuItem>();

                if (SeasonsScreen.seasonSelected.registrationData.opened)
                {
                    if (SeasonsScreen.seasonSelected.userRegistered)
                    {
                        menuItems.Add(new HomeMenuItem { Id = MenuItemType.RegisteredToSeason, Title = "Nadchodzący sezon" });
                    }
                    else
                    {
                        menuItems.Add(new HomeMenuItem { Id = MenuItemType.RegisterToSeason, Title = "Rejestracja" });
                    }
                    
                }
                else
                {
                    menuItems.Add(new HomeMenuItem { Id = MenuItemType.Leaderboards, Title = "Ranking" });
                    menuItems.Add(new HomeMenuItem { Id = MenuItemType.TeamLeaderboards, Title = "Ranking drużyn" });
                    if (!SeasonsScreen.seasonSelected.userRegistered && !(SeasonsScreen.seasonSelected.finishedRaces >= SeasonsScreen.seasonSelected.racesCount))
                    {
                        menuItems.Add(new HomeMenuItem { Id = MenuItemType.RegisterToSeason, Title = "Rejestracja" });
                    }
                    
                }

                menuItems.Add(new HomeMenuItem { Id = MenuItemType.About, Title = "O lidze" });
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.Options, Title = "Opcje" });
            }
            else
            {
                menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem{Id = MenuItemType.Login, Title="Logowanie"},
                    new HomeMenuItem {Id = MenuItemType.About, Title="O lidze" },
                    new HomeMenuItem {Id = MenuItemType.Options, Title="Opcje" }
                };
            }
            if(LoginScreen.admin)
            {
                menuItems.Add(new HomeMenuItem { Id = MenuItemType.ADMIN_Seasons, Title = "[Sezony]" });
            }

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };

            LogOutButton.IsVisible = LoginScreen.loggedIn;
        }

        public async Task UpdateSeasons(bool force = false, bool keepSeason = true)
        {
            if (MainPage.networkData == null) return;

            int idSelected = -1;

            if(SeasonsScreen.seasons == null || force)
            {
                if(SeasonsScreen.seasons != null)
                {
                    idSelected = SeasonsScreen.seasonSelected.id;
                }
                await SeasonsScreen.GetSeasons();
            }
            else
            {
                idSelected = SeasonsScreen.seasonSelected.id;
            }

            SeasonPicker.Items.Clear();

            for (int i = 0; i < SeasonsScreen.seasons.Count; i++)
            {
                SeasonPicker.Items.Add("Sezon " + SeasonsScreen.seasons[i].id.ToString());
            }

            if(idSelected != -1 && keepSeason)
            {
                for(int i = 0;i<SeasonsScreen.seasons.Count;i++)
                {
                    if(SeasonsScreen.seasons[i].id == idSelected)
                    {
                        SeasonsScreen.seasonSelected = SeasonsScreen.seasons[i];
                        SeasonPicker.SelectedIndex = SeasonsScreen.seasonSelected.id;
                        break;
                    }
                }
            }
            else if(keepSeason)
            {
                for (int i = 0; i < SeasonsScreen.seasons.Count; i++)
                {
                    if (SeasonsScreen.seasons[i] == SeasonsScreen.seasonSelected)
                    {
                        SeasonPicker.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        async void LogOutButton_ClickedAsync()
        {
            LoginScreen loginScreen = (LoginScreen)((MainPage)Application.Current.MainPage).GetLoadedPage(MenuItemType.Login);
            if (loginScreen == null) // Login screen is not loaded
            {
                await MainPage.singleton.NavigateFromMenu((int)MenuItemType.Login);
                loginScreen = (LoginScreen)((MainPage)Application.Current.MainPage).GetLoadedPage(MenuItemType.Login);
            }
            loginScreen.LogOut();

            Environment.Exit(1);
        }

        private void LogOutButton_Clicked(object sender, EventArgs e)
        {
            LogOutButton_ClickedAsync();
        }

        private void SeasonPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SeasonsScreen.SelectSeason(SeasonPicker.SelectedIndex);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            DisplayAlert("? 2 ?? :)", "W pierwszym sezonie w historii się na nim ścigają", "Yay, ale jeszcze kilka zostało");
        }
    }
}