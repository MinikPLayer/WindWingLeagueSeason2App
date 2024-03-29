﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace WindWingLeagueSeason2App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class InitScreen : ContentPage
    {
        public bool pageInFocus = false;

        public InitScreen()
        {
            InitializeComponent();

            Title = "Login";
        }

        async Task AnimateLoginText()
        {
            int counter = 0;
            while(pageInFocus)
            {
                string text = "Logowanie";
                for(int i = 0;i<counter;i++)
                {
                    text += ".";
                }
                LoginText.Text = text;
                counter = (counter + 1) % 4;
                await Task.Delay(1000);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pageInFocus = true;
            AnimateLoginText();
            WaitForConnectionAndLogin();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pageInFocus = false;
        }

        async Task WaitForConnectionAndLogin()
        {
            while(!MainPage.networkData.connected)
            {
                await Task.Delay(100);
            }

            if (MainPage.config.autoLogin)
            {
                LoginT(MainPage.config.autoLoginLogin, MainPage.config.autoLoginToken);
                return;
            }
            else
            {
                MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Login, (int)Models.MenuItemType.InitScreen);
            }
        }

        public async static Task<bool> LoginT(string login, string token)
        {
            string data = await MainPage.networkData.RequestAsync("LoginT;" + login + ";" + token.Replace(";", "\\:"));
            string[] datas = data.Split(';');
            if (datas[0] == "OK" && datas.Length > 1)
            {
                LoginScreen.username = datas[1];
                LoginScreen.token = token;

                LoginScreen.loggedIn = true;

                if (datas.Length > 2)
                {
                    if (datas[2] == "True")
                    {
                        LoginScreen.admin = true;
                    }
                }
                MenuPage.actualMenu.UpdateItems();

                return true;
            }
            else
            {
                if (datas[0].Length == 0 || datas[0] == "NC") // Not connected or bad packet, try again
                {
                    return await LoginT(login, token);

                }

                Debug.Log("LoginT error");

                if (data.Length > 1)
                {
                    LoginScreen.loginError = datas[1];
                }
                else
                {
                    LoginScreen.loginError = "Nieznany błąd logowania: " + datas[0];
                }

                await MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Login, (int)Models.MenuItemType.InitScreen);
                return false;
            }
        }
    }
}