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
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            Title = "Rejestracja";

            BindingContext = this;
        }

        async void Register(string login, string password, string email, string steamlink)
        {
            string result = await MainPage.networkData.RequestAsync("RegisterUser;" + login + ";" + MUtil.HashSHA256(password, login).Replace(";", "\\:") + ";" + email + ";" + steamlink);
            string[] results = result.Split(';');
            if (results[0] != "OK")
            {
                if (results.Length > 2)
                {
                    bool state = await DisplayAlert("Błąd rejestracji", results[1], "Więcej informacji", "OK");
                    if(state)
                    {
                        await DisplayAlert("Błąd rejestracji", results[2], "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Błąd rejestracji", results[1], "OK");
                }
            }
            else
            {
                DisplayAlert("Sukces!", "Rejestracja przebiegła pomyślnie", "OK");
                LoginScreen.setLogin = login;
                LoginScreen.setPassword = password;

                Debug.Log("Token: \"" + results[1].Replace("\\:",";") + "\"");
                await MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Login, (int)Models.MenuItemType.Register);
            }
        }

        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            if(LoginEntryBox.Text == null || LoginEntryBox.Text.Length == 0)
            {
                DisplayAlert("Uzupełnij wszystkie pola", "Pole login musi być uzupełnione", "OK");
                return;
            }
            if (PasswordEntryBox.Text == null || PasswordEntryBox.Text.Length == 0)
            {
                DisplayAlert("Uzupełnij wszystkie pola", "Pole hasło musi być uzupełnione", "OK");
                return;
            }
            if (EmailEntryBox.Text == null || EmailEntryBox.Text.Length == 0)
            {
                DisplayAlert("Uzupełnij wszystkie pola", "Pole steam link musi być uzupełnione", "OK");
                return;
            }
            if (SteamLinkEntryBox.Text == null || SteamLinkEntryBox.Text.Length == 0)
            {
                DisplayAlert("Uzupełnij wszystkie pola", "Pole steam link musi być uzupełnione", "OK");
                return;
            }
            if(PasswordRepeatEntryBox.Text == null || PasswordRepeatEntryBox.Text.Length == 0)
            {
                DisplayAlert("Uzupełnij wszystkie pola", "Dla potwierdzenia wpisz hasło 2 razy", "OK");
                return;
            }

            if(PasswordRepeatEntryBox.Text != PasswordEntryBox.Text)
            {
                DisplayAlert("Błąd haseł", "Hasła nie są identyczne", "OK");
                return;
            }

            Register(LoginEntryBox.Text, PasswordEntryBox.Text, EmailEntryBox.Text, SteamLinkEntryBox.Text);
        }
    }
}