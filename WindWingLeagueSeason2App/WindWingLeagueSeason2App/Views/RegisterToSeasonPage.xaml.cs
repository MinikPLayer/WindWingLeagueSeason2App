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
    public partial class RegisterToSeasonPage : ContentPage
    {


        public RegisterToSeasonPage()
        {
            InitializeComponent();

            Title = "Rejestracja";

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id;
            RegistrationTrack.Text = "Tor: " + SeasonsScreen.seasonSelected.registrationTrack.country;
        }

        async void RegisterToSeason(TimeSpan lapDry, TimeSpan lapWet, string dryLink, string wetLink)
        {
            string response = await MainPage.networkData.RequestAsync("RegisterSeason;" + SeasonsScreen.seasonSelected.id.ToString() + ";" + lapDry.ToString() + ";" + lapWet.ToString() + ";" + dryLink + ";" + wetLink);

            if (response == "OK")
            {
                DisplayAlert("Zarejestrowano", "Zarejestrowano pomyślnie do sezonu", "OK");

                MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.RegisteredToSeason, (int)Models.MenuItemType.RegisterToSeason);

                MenuPage.actualMenu.UpdateItems();
            }
            else
            {
                string[] data = response.Split(';');
                if (data.Length > 1)
                {
                    DisplayAlert("Błąd rejestracji", data[1], "OK");
                }
                else
                {
                    DisplayAlert("Błąd rejestracji", data[0], "OK");
                }
            }
        }

        bool ParseTime(string str, out TimeSpan time)
        {
            try
            {
                char splitChar = ':';
                if (str.Contains('.'))
                {
                    splitChar = '.';
                }

                string[] data = str.Split(splitChar);
                if (data.Length != 3)
                {
                    Debug.LogError("Error parsing time, bad data count");
                    return false;
                }

                while(data[2].Length < 3)
                {
                    data[2] += "0";
                }

                Debug.Log("Minutes: " + data[0] + ", seconds: " + data[1] + ", miliseconds: " + data[2]);

                time = new TimeSpan(0, 0, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]));
                return true;
            }
            catch(FormatException e)
            {
                Debug.LogError("[RegisterToSeasonPage.ParseTime] Error parsing time: error parsing numbers");
                return false;
            }
            catch(Exception e)
            {
                Debug.Exception(e, "[RegisterToSeasonPage.ParseTime]");
                return false;
            }
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            if (!ParseTime(DryTimeBox.Text, out TimeSpan dryTime))
            {
                DisplayAlert("Błąd", "Błędny format czasu na suchym torze\nPoprawny format: MM:SS:MSS [np. 1:23:456] lub MM.SS.MSS [np.1.23.456]", "OK");
                return;
            }

            if (!ParseTime(WetTimeBox.Text, out TimeSpan wetTime))
            {
                DisplayAlert("Błąd", "Błędny format czasu na mokrym torze\nPoprawny format: MM:SS:MSS [np. 1:23:456] lub MM.SS.MSS [np.1.23.456]", "OK");
                return;
            }
            RegisterToSeason(dryTime, wetTime, DryLinkBox.Text, WetLinkBox.Text);
        }
    }
}