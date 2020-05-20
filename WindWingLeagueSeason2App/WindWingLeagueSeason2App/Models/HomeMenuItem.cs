using System;
using System.Collections.Generic;
using System.Text;

namespace WindWingLeagueSeason2App.Models
{
    public enum MenuItemType
    {
        Login,
        Leaderboards,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
