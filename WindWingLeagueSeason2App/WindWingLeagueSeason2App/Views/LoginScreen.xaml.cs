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
        public LoginScreen()
        {
            InitializeComponent();

            Title = "Login";
        }

        async Task Login(string login, string password)
        {
            string data = await MainPage.networkData.RequestAsync("Login;" + login + ";" + password);
            string[] datas = data.Split(';');
            if (datas[0] == "OK" && datas.Length > 1)
            {
                username = datas[1];

                loggedIn = true;

                MenuPage.actualMenu.UpdateItems();
            }
            else
            {
                LoginInfo.Text = "Login error, code: " + datas[0];
            }

            LoginButton.IsEnabled = true;
        }

        public void LogOut()
        {
            if (loggedIn)
            {

                loggedIn = false;

                MenuPage.actualMenu.UpdateItems();

                LoginBox.Text = "";
                PasswordBox.Text = "";
            }
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;

            Login(LoginBox.Text, PasswordBox.Text);
        }
    }
}