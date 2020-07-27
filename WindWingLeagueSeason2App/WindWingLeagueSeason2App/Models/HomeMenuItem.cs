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
        TeamLeaderboards,
        Options,
        Register,
        RegisterToSeason,
        RegisteredToSeason,
        DriverInfo,
        About,
        ADMIN_Users,
        ADMIN_Seasons,
        ADMIN_SeasonUser,
        ADMIN_SaveSeason,
        ADMIN_CreateSeason,
        ADMIN_RaceResults,
        ADMIN_RegisterSeasonUser,
        ADMIN_RegisterSeasonUsersSelector
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }

}
