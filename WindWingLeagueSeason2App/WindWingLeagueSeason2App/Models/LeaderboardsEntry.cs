using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App.Models
{
    public class LeaderboardsEntry : INotifyPropertyChanged
    {
        string _prefix;
        public string prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("name"));
                }
            }
        }

        public string rawName
        {
            get
            {
                return _name;
            }
        }

        string _name;
        public string name { 
            get { return prefix + _name; } 
            set
            {
                _name = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("name"));
                }
            } 
        }

        int _score;
        public int score
        {
            get { return _score; }
            set
            {
                _score = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("score"));
                }
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
                return highlighted ? (Color)Application.Current.Resources["NavigationPrimary"] : Color.White;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
