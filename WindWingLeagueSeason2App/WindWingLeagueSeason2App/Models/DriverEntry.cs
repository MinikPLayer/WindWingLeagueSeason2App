using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App.Models
{
    public class DriverEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public User user;
        public Season.SeasonUser seasonUser;

        //string _name = "";
        public string name
        {
            get
            {
                //return _name;
                
                return user.login;
            }
            /*set
            {
                _name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("name"));
                }
            }*/
        }

        public int points
        {
            get
            {
                return 0;
            }
        }

        bool _highlighted = false;
        public bool highlighted
        {
            get
            {
                return _highlighted;
            }

            set
            {
                _highlighted = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("color"));
                }
            }
        }
        public Color color
        {
            get
            {
                return highlighted ? (Color)Application.Current.Resources["DynamicNavigationPrimary"] : (Color)Application.Current.Resources["DynamicBackgroundColor"];
            }
        }

    }
}
