using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WindWingLeagueSeason2App;

using WindWingLeagueSeason2App.Models;
using System.Threading;
using System.Linq;

namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public static MainPage singleton;
        public static NetworkData networkData;
        public static Config config;
        public static string log = "";

        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            singleton = this;
            config = Config.Load();

            ThemeManager.darkMode = config.darkMode;

            MasterBehavior = MasterBehavior.Popover;

            //MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
            if (networkData == null)
            {
                if (App.debug)
                {
                    //networkData = new NetworkData("192.168.1.103", 8148);
                    networkData = new NetworkData("minik.ml", 8148);
                }
                else
                {
                    networkData = new NetworkData("minik.ml", 8148);
                }
            }

            MenuPages.Add((int)MenuItemType.InitScreen, (NavigationPage)Detail);
        }

        public void CloseAllPages()
        {
            lock (MenuPages)
            {
                int id = -1;
                foreach(int entry in MenuPages.Keys.ToList())
                {

                    if(MenuPages[entry] == Detail)
                    {
                        id = entry;
                    }
                    MenuPages.Remove(entry);
                }

                if (id != -1)
                {
                    Debug.Log("[MainPage.CloseAllPages] Loading page with id " + id.ToString());
                    NavigateFromMenu(id);
                }
                else
                {
                    Debug.Log("[MainPage.CloseAllPages] Loading leaderboards page");
                    NavigateFromMenu((int)MenuItemType.Leaderboards);
                }
            }
        }

        public Page GetLoadedPage(MenuItemType type)
        {
            if(!MenuPages.ContainsKey((int)type))
            {
                return null;
            }
            return MenuPages[(int)type].CurrentPage;
        }


        public NavigationPage currentPage;
        public async Task NavigateFromMenu(int id, int pageIdToClose = -1)
        {

            if(pageIdToClose > 0)
            {
                MenuPages.Remove(pageIdToClose);
            }

            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Leaderboards:
                        MenuPages.Add(id, new NavigationPage(new LeaderboardsPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.Login:
                        MenuPages.Add(id, new NavigationPage(new LoginScreen()));
                        break;
                    case (int)MenuItemType.Register:
                        MenuPages.Add(id, new NavigationPage(new RegisterPage()));
                        break;
                    case (int)MenuItemType.Options:
                        MenuPages.Add(id, new NavigationPage(new OptionsPage()));
                        break;
                    case (int)MenuItemType.RegisterToSeason:
                        MenuPages.Add(id, new NavigationPage(new RegisterToSeasonPage()));
                        break;
                    case (int)MenuItemType.RegisteredToSeason:
                        MenuPages.Add(id, new NavigationPage(new UserRegisteredPage()));
                        break;
                    case (int)MenuItemType.DriverInfo:
                        MenuPages.Add(id, new NavigationPage(new DriverInfoPage()));
                        break;
                    case (int)MenuItemType.ADMIN_Seasons:
                        MenuPages.Add(id, new NavigationPage(new Admin.SeasonsPage()));
                        break;
                    case (int)MenuItemType.ADMIN_SaveSeason:
                        MenuPages.Add(id, new NavigationPage(new Admin.SeasonSavePage()));
                        break;
                    case (int)MenuItemType.ADMIN_SeasonUser:
                        MenuPages.Add(id, new NavigationPage(new Admin.Admin_UserPage()));
                        break;
                    case (int)MenuItemType.ADMIN_RaceResults:
                        MenuPages.Add(id, new NavigationPage(new Admin_RaceResultsPage()));
                        break;
                    case (int)MenuItemType.ADMIN_RegisterSeasonUser:
                        MenuPages.Add(id, new NavigationPage(new Admin_RegisterUserToSeason()));
                        break;
                    case (int)MenuItemType.ADMIN_RegisterSeasonUsersSelector:
                        MenuPages.Add(id, new NavigationPage(new Admin_RegisterUserToSeasonSelector()));
                        break;
                    case (int)MenuItemType.ADMIN_Users:
                        return;
                }
            }

            currentPage = MenuPages[id];

            if (currentPage != null && Detail != currentPage)
            {
                Detail = currentPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}