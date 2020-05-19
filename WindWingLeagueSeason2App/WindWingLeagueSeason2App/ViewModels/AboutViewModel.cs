using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

using WindWingLeagueSeason2App.Views;


namespace WindWingLeagueSeason2App.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "O lidze";
            OpenWebCommand = new Command<string>(async (address) => await Browser.OpenAsync(address));
        }



        public ICommand OpenWebCommand { get; }
    }
}