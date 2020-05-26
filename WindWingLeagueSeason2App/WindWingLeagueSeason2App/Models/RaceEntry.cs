using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App.Models
{
    public class RaceEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Race _race;
        public Race race
        {
            get
            {
                return _race;
            }
            set
            {
                if(value == null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("name"));
                    PropertyChanged(this, new PropertyChangedEventArgs("date"));
                    return;
                }

                Race oldRace = _race;
                _race = value;
                if (oldRace != null)
                {
                    if(oldRace.track != value.track)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("name"));
                    }

                    if(oldRace.date != value.date)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("date"));
                    }
                }
            }
        }

        public DateTime date
        {
            get
            {
                return _race.date;
            }
            set
            {
                if (_race == null) return;

                _race.date = value;
            }
        }

        public string name
        {
            get
            {
                return (_race == null) ? "" : _race.track.country;
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
