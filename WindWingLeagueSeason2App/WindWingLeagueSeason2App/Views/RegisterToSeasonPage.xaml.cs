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

            GetRegistrationData();
        }

        async void GetRegistrationData()
        {
            string data = await MainPage.networkData.RequestAsync("RD"); // Registration data

            try
            {
                int count = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == '{')
                    {
                        count = int.Parse(data.Substring(0, i));
                        Debug.Log("Count: " + count);
                        
                    }
                }

                if(count == 0)
                {
                    await DisplayAlert("Brak otwartej rejestracji", "Aktualnie brak otwartych sezonów do rejestracji", "OK");
                    await MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Login, (int)Models.MenuItemType.Register);
                }
            }
            catch(Exception e)
            {
                if(await DisplayAlert("Coś poszło nie tak", "Błąd przetwarzania danych z servera: " + e.Message, "Więcej informacji", "OK"))
                {
                    await DisplayAlert("Więcej informacji", e.ToString(), "OK");
                }

                await MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Login, (int)Models.MenuItemType.Register);
            }
        }

    }
}