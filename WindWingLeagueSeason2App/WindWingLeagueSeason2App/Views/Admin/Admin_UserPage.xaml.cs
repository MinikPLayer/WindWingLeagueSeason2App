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

namespace WindWingLeagueSeason2App.Views.Admin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Admin_UserPage : ContentPage
    {

        public static Season.SeasonUser user;
        public ObservableCollection<string> teams { get; set; }

        public Admin_UserPage()
        {
            InitializeComponent();

            Title = "[ADMIN] Użytkownik";

            teams = new ObservableCollection<string>();

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(user == null)
            {
                DisplayAlert("Błąd", "Nie wybrano użytkownika", "OK");
                MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_SeasonUser);
                return;
            }

            teams.Clear();
            for(int i = 0;i<Team.teams.Length;i++)
            {
                teams.Add(Team.teams[i].name);
            }

            SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id.ToString();
            UsernameText.Text = user.user.login;

            DryTimeText.Text = user.lapDry.ToString("mm':'ss':'fff");
            WetTimeText.Text = user.lapWet.ToString("mm':'ss':'fff");
            TeamText.SelectedItem = user.team.name;
            Team1Text.SelectedItem = user.prefferedTeams[0].name;
            Team2Text.SelectedItem = user.prefferedTeams[1].name;
            Team3Text.SelectedItem = user.prefferedTeams[2].name;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.ADMIN_Seasons, (int)MenuItemType.ADMIN_SeasonUser);
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
            catch(Exception e)
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

        private void DryLinkButton_Clicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(user.lapDryLink);
            OpenLink(user.lapDryLink);
        }

        private void WetLinkButton_Clicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(user.lapWetLink);
            OpenLink(user.lapWetLink);
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (!ParseTime(DryTimeText.Text, out TimeSpan dryTime))
            {
                DisplayAlert("Błąd", "Błędny format czasu na suchym torze\nPoprawny format: MM:SS:MSS [np. 1:23:456] lub MM.SS.MSS [np.1.23.456]", "OK");
                return;
            }

            if (!ParseTime(WetTimeText.Text, out TimeSpan wetTime))
            {
                DisplayAlert("Błąd", "Błędny format czasu na mokrym torze\nPoprawny format: MM:SS:MSS [np. 1:23:456] lub MM.SS.MSS [np.1.23.456]", "OK");
                return;
            }

            if (TeamText.SelectedItem == null || Team1Text.SelectedItem == null || Team2Text.SelectedItem == null || Team3Text.SelectedItem == null)
            {
                DisplayAlert("Błąd", "Musisz wybrać 3 preferowane zespoły", "OK");
                return;
            }

            user.lapDry = dryTime;
            user.lapWet = wetTime;
            user.team = Team.GetTeam(TeamText.SelectedIndex);
            user.prefferedTeams[0] = Team.GetTeam(Team1Text.SelectedIndex);
            user.prefferedTeams[1] = Team.GetTeam(Team2Text.SelectedIndex);
            user.prefferedTeams[2] = Team.GetTeam(Team3Text.SelectedIndex);

            BackButton_Clicked(sender, null);
        }

        private void SteamLinkButton_Clicked(object sender, EventArgs e)
        {
            string link = user.user.steam;
            if(link.StartsWith("steam"))
            {
                link = "https://" + link;
            }
            Clipboard.SetTextAsync(link);
            OpenLink(link);
        }
    }
}