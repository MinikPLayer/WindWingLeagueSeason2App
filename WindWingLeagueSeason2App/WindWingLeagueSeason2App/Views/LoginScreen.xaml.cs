using System;
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
    public partial class LoginScreen : ContentPage
    {
        public static string username;
        public static bool loggedIn = false;

        public static string loginError = "";

        public static string setLogin = "";
        public static string setPassword = "";

        public LoginScreen()
        {
            InitializeComponent();

            Title = "Login";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(loginError.Length > 0)
            {
                LoginInfo.TextColor = Color.Red;
                LoginInfo.Text = loginError;
                loginError = "";
                return;
            }

            if (setLogin.Length > 0)
            {
                LoginBox.Text = setLogin;
                setLogin = "";
            }
            if(setPassword.Length > 0)
            {
                PasswordBox.Text = setPassword;
                setPassword = "";
            }

            
        }

        async Task LoginT(string login, string token)
        {
            string data = await MainPage.networkData.RequestAsync("LoginT;" + login + ";" + token.Replace(";", "\\:"));
            string[] datas = data.Split(';');
            if (datas[0] == "OK" && datas.Length > 1)
            {
                username = datas[1];

                loggedIn = true;

                MenuPage.actualMenu.UpdateItems();

                LoginInfo.Text = "";
                LoginInfo.TextColor = Color.White;
            }
            else
            {
                LoginInfo.TextColor = Color.Red;
                if (datas.Length > 1)
                    LoginInfo.Text = datas[1];
                else
                    LoginInfo.Text = datas[0];

            }

            LoginButton.IsEnabled = true;
        }

        async Task Login(string login, string password, bool autoLogin)
        {
            string data = await MainPage.networkData.RequestAsync("Login;" + login + ";" + MUtil.HashSHA256(password, login).Replace(";", "\\:"));
            string[] datas = data.Split(';');
            if (datas[0] == "OK" && datas.Length > 1)
            {
                username = datas[1];

                loggedIn = true;

                if(datas.Length > 2)
                {
                    Debug.Log("Token: \"" + datas[2].Replace("\\:",";") + "\"");

                    if(autoLogin)
                    {
                        MainPage.config.autoLogin = true;
                        MainPage.config.autoLoginLogin = login;
                        MainPage.config.autoLoginToken = datas[2].Replace("\\:", ";");

                        Config.Save();
                    }
                }

                MenuPage.actualMenu.UpdateItems();

                LoginInfo.Text = "";
                LoginInfo.TextColor = Color.White;
            }
            else
            {
                LoginInfo.TextColor = Color.Red;
                if (datas.Length > 1) 
                    LoginInfo.Text = datas[1];
                else
                    LoginInfo.Text = datas[0];

            }

            LoginButton.IsEnabled = true;
        }

        public void LogOut()
        {
            if (loggedIn)
            {
                MainPage.config.autoLogin = false;
                Config.Save();

                loggedIn = false;

                MenuPage.actualMenu.UpdateItems();

                LoginBox.Text = "";
                PasswordBox.Text = "";
            }
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;
            LoginInfo.Text = "";

            Login(LoginBox.Text, PasswordBox.Text, AutoLoginSwitch.IsChecked);
        }
        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            MainPage.singleton.NavigateFromMenu((int)Models.MenuItemType.Register);
        }

        private void AutoLoginSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}