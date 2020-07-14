using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WindWingLeagueSeason2App.Models;

namespace WindWingLeagueSeason2App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRegisteredPage : ContentPage
    {

        Season.SeasonUser user;

        public UserRegisteredPage()
        {
            InitializeComponent();

            Title = "Zarejestrowano";

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetSeasonUserData();
        }

        async void GetSeasonUserData()
        {
            string[] data = null;
            try
            {
                data = (await MainPage.networkData.RequestAsync("SeasonUser;" + SeasonsScreen.seasonSelected.id.ToString() + ";Get;C")).Split(';');
                if (data[0] == "OK")
                {
                    user = new Season.SeasonUser();
                    bool result = await user.Deserialize(data[1]);
                    if (!result)
                    {
                        await DisplayAlert("Błąd", "Nie udało się przetworzyć danych użytkownika, możliwy błąd servera, pakiet:\n" + data[1], "OK");
                        await MainPage.singleton.NavigateFromMenu((int)MenuItemType.About);
                        MainPage.singleton.CloseAllPages();
                    }
                }
                else
                {
                    if (data.Length > 0)
                    {
                        DisplayAlert("Błąd", data[1], "OK");
                    }
                    else
                    {
                        DisplayAlert("Błąd", data[0], "OK");
                    }

                }

                SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id;
                if (SeasonsScreen.seasonSelected.races.Count > 0)
                {
                    FirstRaceLocation.Text = SeasonsScreen.seasonSelected.races[0].track.country;
                    FirstRaceDate.Text = SeasonsScreen.seasonSelected.races[0].date.ToString(new CultureInfo("de-DE"));
                }

                DryTimeText.Text = user.lapDry.ToString("mm':'ss':'fff");
                WetTimeText.Text = user.lapWet.ToString("mm':'ss':'fff");
                Team1Text.Text = user.prefferedTeams[0].name;
                Team2Text.Text = user.prefferedTeams[1].name;
                Team3Text.Text = user.prefferedTeams[2].name;
            }
            catch(Exception e)
            {
                bool result = await DisplayAlert("Błąd", "Błąd przetwarzania danych", "Więcej informacji", "OK");
                if(result)
                {
                    await DisplayAlert("Więcej informacji", e.ToString(), "OK");
                    DisplayAlert("Pakiet", data[1], "OK");
                }
            }
        }

        private void CorrectInfoButton_Clicked(object sender, EventArgs e)
        {
            MainPage.singleton.NavigateFromMenu((int)MenuItemType.RegisterToSeason, (int)MenuItemType.RegisteredToSeason);
        }
    }
}