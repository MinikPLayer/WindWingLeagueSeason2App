using WindWingLeagueSeason2App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        public static MenuPage actualMenu;

        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            UpdateItems();

            actualMenu = this;
        }

        public void UpdateItems(bool updateSeasons = true)
        {
            if(LoginScreen.loggedIn)
            {
                menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem{Id = MenuItemType.Leaderboards, Title = "Ranking"},
                    new HomeMenuItem {Id = MenuItemType.About, Title="O lidze" },
                    new HomeMenuItem {Id = MenuItemType.Options, Title="Opcje" }
                };
            }
            else
            {
                menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem{Id = MenuItemType.Login, Title="Login"},
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

            if (updateSeasons)
            {
                UpdateSeasons();
            }


            LogOutButton.IsVisible = LoginScreen.loggedIn;
        }

        public async Task UpdateSeasons(bool force = false)
        {
            if (MainPage.networkData == null) return;

            if(SeasonsScreen.seasons == null || force)
            {
                await SeasonsScreen.GetSeasons();
            }

            SeasonPicker.Items.Clear();

            for (int i = 0; i < SeasonsScreen.seasons.Count; i++)
            {
                SeasonPicker.Items.Add("Sezon " + SeasonsScreen.seasons[i].id.ToString());
            }

            SeasonPicker.SelectedItem = SeasonsScreen.seasonSelected;
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
        }

        private void LogOutButton_Clicked(object sender, EventArgs e)
        {
            LogOutButton_ClickedAsync();
        }

        private void SeasonPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SeasonsScreen.SelectSeason(SeasonPicker.SelectedIndex);
        }
    }
}