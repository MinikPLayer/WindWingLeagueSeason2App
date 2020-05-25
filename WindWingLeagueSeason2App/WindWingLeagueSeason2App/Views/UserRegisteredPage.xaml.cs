using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindWingLeagueSeason2App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRegisteredPage : ContentPage
    {


        public UserRegisteredPage()
        {
            InitializeComponent();

            Title = "Zarejestrowano";

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            SeasonName.Text = "Sezon " + SeasonsScreen.seasonSelected.id;
            if(SeasonsScreen.seasonSelected.races.Count > 0)
            {
                FirstRaceLocation.Text = SeasonsScreen.seasonSelected.races[0].track.country;
                FirstRaceDate.Text = SeasonsScreen.seasonSelected.races[0].date.ToString(new CultureInfo("de-DE"));
            }
        }
    }
}