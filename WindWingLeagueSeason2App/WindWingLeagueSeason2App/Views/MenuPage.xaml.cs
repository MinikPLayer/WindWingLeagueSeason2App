﻿using WindWingLeagueSeason2App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        public void UpdateItems()
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

        private void LogOutButton_Clicked(object sender, EventArgs e)
        {
            LoginScreen loginScreen = (LoginScreen)((MainPage)Application.Current.MainPage).GetLoadedPage(MenuItemType.Login);
            loginScreen.LogOut();
        }
    }
}