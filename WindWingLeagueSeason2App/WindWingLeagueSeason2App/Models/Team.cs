using System;
using System.Collections.Generic;
using System.Text;
using WindWingLeagueSeason2App.Views;

namespace WindWingLeagueSeason2App.Models
{
    public class Team
    {
        public int id;
        string _name;
        public string shortName;
        public string iconPath;

        public string name
        {
            get
            {
                return GetName();
            }
        }

        public Team(int id, string name, string shortName, string iconPath)
        {
            this.id = id;
            this._name = name;
            this.shortName = shortName;
            this.iconPath = iconPath;
        }

        public string GetName(int gameVersion = -1)
        {
            if(gameVersion == -1)
            {
                if(SeasonsScreen.seasonSelected == null)
                {
                    gameVersion = 2019;
                }
                else
                {
                    gameVersion = SeasonsScreen.seasonSelected.gameVersion;
                }
            }

            if (gameVersion == 2020 && id == 8) // Toro Rosso
            {
                return "Alpha Tauri";
            }

            return _name;
        }

        public static string GetName(int id, int gameVersion = -1)
        {
            return GetTeam(id).GetName(gameVersion);
        }

        public static Team GetTeam(int id)
        {
            if (id < 0 || id >= teams.Length) return null;
            return teams[id];
        }

        public static Team GetTeamShort(string shortName)
        {
            for (int i = 0; i < teams.Length; i++)
            {
                if (teams[i].shortName == shortName)
                {
                    return teams[i];
                }
            }
            return null;
        }

        public static Team GetTeam(string name)
        {
            for (int i = 0; i < teams.Length; i++)
            {
                if (teams[i].name == name)
                {
                    return teams[i];
                }
            }
            return null;
        }

        public static Team[] teams = new Team[] {
            new Team(0, "Mercedes", "MER", ""),
            new Team(1, "Ferrari", "FER", ""),
            new Team(2, "Red Bull", "RDB", ""),
            new Team(3, "Renault", "REN", ""),
            new Team(4, "Haas", "HAS", ""),
            new Team(5, "McLaren", "MCL", ""),
            new Team(6, "Racing Point", "RPT", ""),
            new Team(7, "Alfa Romeo", "ARM", ""),
            new Team(8, "Toro Rosso", "TRS", ""),
            new Team(9, "Williams", "WIL", ""),
            new Team(10, "Niesprecyzowany", "OTH", "")
        };

        public static Team other = teams[10];
    }
}
