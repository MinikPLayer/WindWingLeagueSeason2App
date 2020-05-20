using System;
using System.ComponentModel;
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

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            LoginInfo.Text = "\nLogin: " + LoginBox.Text + "\nPassword: " + PasswordBox.Text;

            username = LoginBox.Text;

            loggedIn = true;

            MenuPage.actualMenu.UpdateItems();

            
        }
    }
}