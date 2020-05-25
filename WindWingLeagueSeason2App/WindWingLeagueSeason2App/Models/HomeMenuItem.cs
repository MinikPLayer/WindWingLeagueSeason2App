using System;
using System.Collections.Generic;
using System.Text;

namespace WindWingLeagueSeason2App.Models
{
    public enum MenuItemType
    {
        InitScreen,
        Login,
        Leaderboards,
        Options,
        Register,
        RegisterToSeason,
        RegisteredToSeason,
        About,
        ADMIN_Users,
        ADMIN_Seasons,
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }

}
