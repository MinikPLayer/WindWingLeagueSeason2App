﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WindWingLeagueSeason2App.Models
{
    public class Season
    {
        public class SeasonUser
        {
            public User user;
            public int id;
            public TimeSpan lapDry;
            public TimeSpan lapWet;
            public string lapDryLink;
            public string lapWetLink;
            public int priority;
            public Team team;
            public Team[] prefferedTeams;

            public void LoadDefaults()
            {
                this.lapDry = new TimeSpan(0, 0, 0);
                this.lapWet = new TimeSpan(0, 0, 0);
                this.lapDryLink = "";
                this.lapWetLink = "";
                this.priority = int.MaxValue;

                //this.team = Team.GetTeam("Other");
                this.team = Team.other;

                this.prefferedTeams = new Team[3];
                for(int i = 0;i<3;i++)
                {
                    //this.prefferedTeams[i] = Team.GetTeam("Other");
                    this.prefferedTeams[i] = Team.other;
                }
            }

            public SeasonUser()
            {
                LoadDefaults();
            }

            public SeasonUser(User user)
            {
                this.user = user;
            }

            public SeasonUser(User user, TimeSpan lapDry, TimeSpan lapWet, string lapDryLink, string lapWetLink, int priority = int.MaxValue)
            {
                this.user = user;

                FillVariables(lapDry, lapWet, lapDryLink, lapWetLink, priority);
            }

            async Task<bool> ParseSinglePacket(string header, string content)
            {
                try
                {
                    switch (header)
                    {
                        case "id":
                            id = int.Parse(content);
                            user = await User.GetUser(id);
                            return true;

                        case "lapDry":
                            lapDry = TimeSpan.ParseExact(content, "mm':'ss':'fff", null);
                            return true;

                        case "lapWet":
                            lapWet = TimeSpan.ParseExact(content, "mm':'ss':'fff", null);
                            return true;

                        case "lapDryLink":
                            lapDryLink = content;
                            return true;

                        case "lapWetLink":
                            lapWetLink = content;
                            return true;

                        case "team":
                            team = Team.GetTeam(int.Parse(content));
                            return true;

                        case "team1":
                            prefferedTeams[0] = Team.GetTeam(int.Parse(content));
                            return true;

                        case "team2":
                            prefferedTeams[1] = Team.GetTeam(int.Parse(content));
                            return true;

                        case "team3":
                            prefferedTeams[2] = Team.GetTeam(int.Parse(content));
                            return true;

                        default:
                            Debug.LogError("[Season.SeasonUser.ParseSinglePacket] Unknown header");
                            return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.Exception(e, "[Season.SeasonUser.ParseSinglePacket]");
                    return false;
                }
            }

            public async Task<bool> Deserialize(string str)
            {
                try
                {
                    if (!str.StartsWith("seasonUser{"))
                    {
                        Debug.LogError("[Season.SeasonUser.Deserialize] It's no a race packet, bad magic");
                        return false;
                    }

                    str = str.Substring(0, str.Length - 1).Remove(0, 11); // remove race{ and }

                    List<string> packets = MUtil.SplitWithBrackets(str);
                    for (int i = 0; i < packets.Count; i++)
                    {
                        bool done = false;
                        for (int j = 0; j < packets[i].Length; j++)
                        {
                            if (packets[i][j] == '{')
                            {
                                if (!await ParseSinglePacket(packets[i].Substring(0, j), packets[i].Substring(0, packets[i].Length - 1).Remove(0, j + 1)))
                                {
                                    Debug.LogError("[Season.SeasonUser.Deserialize] Cannot parse packet with header: " + packets[i].Substring(0, j));
                                    return false;
                                }
                                done = true;
                                break;
                            }
                        }
                        if (!done)
                        {
                            Debug.LogError("[Season.SeasonUser.Deserialize] Cannot find a closing bracket, incomplete packet");
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Debug.Exception(e, "[Season.SeasonUser.Deserialize]");
                    return false;
                }
            }

            public string Serialize()
            {
                return "seasonUser{id{" + user.id.ToString() + "},lapDry{" + lapDry.ToString("mm':'ss':'fff") + "},lapWet{" + lapWet.ToString("mm':'ss':'fff") + "},lapDryLink{" + lapDryLink + "},lapWetLink{" + lapWetLink + "},team{" + team.id.ToString() + "},team1{" + prefferedTeams[0].id + "},team2{" + prefferedTeams[1].id + "},team3{" + prefferedTeams[2].id + "}}";
            }



            public void FillVariables(TimeSpan lapDry, TimeSpan lapWet, string lapDryLink, string lapWetLink, int priority = int.MaxValue)
            {
                this.lapDry = lapDry;
                this.lapWet = lapWet;
                this.lapDryLink = lapDryLink;
                this.lapWetLink = lapWetLink;
                this.priority = priority;
            }

        }

        public int id;
        public int racesCount;
        public List<Race> races;
        public List<SeasonUser> users = new List<SeasonUser>();
        public RegistrationData registrationData;
        public int finishedRaces;
        public int gameVersion;
        public bool finished
        {
            get
            {
                return finishedRaces >= racesCount;
            }
        }
        public Track registrationTrack;

        public bool userRegistered;

        public void Log()
        {
            Debug.Log("Season " + id.ToString() + ": ");
            Debug.Log("Races count: " + racesCount);
            Debug.Log("Finished: " + finished);
            if (registrationTrack == null)
            {
                Debug.Log("Registration track: null");
            }
            else
            {
                Debug.Log("Registration track: " + registrationTrack.country);
            }


            Debug.Log("Races (" + races.Count.ToString() + "): ");
            for (int i = 0; i < races.Count; i++)
            {
                races[i].Log();
            }
        }

        public Season(string seasonString, int forceID = -1)
        {
            SetDefaultValues();

            if (forceID > 0)
            {
                this.id = forceID;
            }
            Deserialize(seasonString);
        }

        async Task<bool> ParseSinglePacket(string header, string content)
        {
            try
            {
                switch (header)
                {
                    case "id":
                        id = int.Parse(content);
                        return true;

                    case "finishedRaces":
                        finishedRaces = int.Parse(content);
                        return true;

                    case "registration":
                        registrationData.Deserialize(header + "{" + content + "}");
                        return true;

                    case "gameVersion":
                        gameVersion = int.Parse(content);
                        return true;

                    case "track":
                        if (content.StartsWith("c("))
                        {
                            content = content.Substring(0, content.Length - 1).Remove(0, 2); // remove c( and )
                            registrationTrack = Track.GetTrack(content);

                            return true;
                        }
                        else if (content.StartsWith("id("))
                        {
                            content = content.Substring(0, content.Length - 1).Remove(0, 3); // remove id( and )
                            registrationTrack = Track.GetTrack(int.Parse(content));

                            return true;
                        }
                        else
                        {
                            Debug.LogError("[WindWingApp.Season.ParseSinglePacket] Unknown track header");
                        }
                        return false;

                    case "races":
                        {
                            races.Clear();
                            List<string> packets = MUtil.SplitWithBrackets(content);
                            for (int i = 0; i < packets.Count; i++)
                            {
                                Race r = new Race();
                                if (!await r.ParseRaceString(packets[i]))
                                {
                                    Debug.LogError("[WindWingApp.Season.ParseSinglePacket] Cannot parse race packet");
                                    return false;
                                }
                                races.Add(r);
                            }
                            racesCount = races.Count;
                            return true;
                        }

                    case "users":
                        {
                            users.Clear();
                            List<string> packets = MUtil.SplitWithBrackets(content);
                            for (int i = 0; i < packets.Count; i++)
                            {
                                SeasonUser r = new SeasonUser();
                                if (!await r.Deserialize(packets[i]))
                                {
                                    Debug.LogError("[WindWingApp.Season.ParseSinglePacket] Cannot parse race packet");
                                    return false;
                                }
                                users.Add(r);
                            }
                            return true;
                        }

                    default:
                        Debug.LogError("[WindWingApp.Season.ParseSinglePacket] Unknown header");
                        return false;
                }

            }
            catch (Exception e)
            {
                Debug.LogError("[WindWingApp.Season.ParseSinglePacket] Error parsing packet, exception: " + e.ToString());
                return false;
            }
        }

        public async Task<bool> Deserialize(string str)
        {
            try
            {
                if (!str.StartsWith("Season{"))
                {
                    Debug.LogError("[WindWingApp.Season.ParseSeasonString] Not a season string: bad magic, string:\n" + str);
                    return false;
                }

                str = str.Substring(0, str.Length - 1).Remove(0, 7); // Remove Season{ and } at the end

                List<string> packets = MUtil.SplitWithBrackets(str);

                for (int i = 0; i < packets.Count; i++)
                {
                    bool done = false;
                    for (int j = 0; j < packets[i].Length; j++)
                    {
                        if (packets[i][j] == '{')
                        {
                            if (!await ParseSinglePacket(packets[i].Substring(0, j), packets[i].Substring(0, packets[i].Length - 1).Remove(0, j + 1)))
                            {
                                Debug.LogError("[WindWingApp.Season.ParseSeasonString] Cannot parse packet with header: " + packets[i].Substring(0, j));
                                return false;
                            }
                            done = true;
                            break;
                        }
                    }
                    if (!done)
                    {
                        Debug.LogError("[WindWingApp.Season.ParseSeasonString] Cannot find a closing bracket, incomplete packet");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.Exception(e, "[WindWingAppServer.ParseSeasonString]");
                return false;
            }
        }

        public async Task<SeasonUser> GetUser(int id)
        {
            for(int i = 0;i<users.Count;i++)
            {
                if(users[i].user.id == id)
                {
                    return users[i];
                }
            }

            string response = await Views.MainPage.networkData.RequestAsync("SeasonUser;" + this.id.ToString() + ";Get;" + id.ToString() + ";");
            var packets = response.Split(';');

            if(packets[0] != "OK" || packets.Length == 1)
            {
                int index = 0;
                if(packets.Length > 1)
                {
                    index = 1;
                }
                await Views.MainPage.singleton.DisplayAlert("Błąd", "Błąd pobierania danych o użytkowniku: " + packets[index], "OK");
                return null; 
            }

            SeasonUser u = new SeasonUser();
            if(!await u.Deserialize(packets[1]))
            {
                await Views.MainPage.singleton.DisplayAlert("Błąd", "Błąd przetwarzania danych o użytkowniku", "OK");
                return null;
            }

            users.Add(u);
            return u;
        }

        public string Serialize()
        {
            string str = "Season{";

            str += "id{" + id.ToString() + "},";
            str += registrationData.Serialize() + ",";
            str += registrationTrack.Serialize() + ",";
            str += "gameVersion{" + gameVersion.ToString() + "},";
            str += "finishedRaces{" + finishedRaces + "}";

            str += ",races{";

            for(int i = 0;i<races.Count;i++)
            {
                str += races[i].Serialize();
                if(i != races.Count - 1)
                {
                    str += ',';
                }
            }

            str += "}";

            str += ",users{";

            for (int i = 0; i < users.Count; i++)
            {
                str += users[i].Serialize();
                if (i != users.Count - 1)
                {
                    str += ',';
                }
            }

            str += "}";

            return str + "}";
        }

        public void SortRaces()
        {
            for (int i = 0; i < races.Count; i++)
            {
                int index = i;
                DateTime min = DateTime.MaxValue;
                for (int j = i; j < races.Count; j++)
                {
                    if (races[j].date < min)
                    {
                        index = j;
                        min = races[j].date;
                    }
                }

                Race pom = races[i];
                races[i] = races[index];
                races[index] = pom;
            }
        }

        void SetDefaultValues()
        {
            this.userRegistered = false;
            this.id = -1;
            this.racesCount = 0;
            this.finishedRaces = 0;

            this.races = new List<Race>(racesCount);

            this.gameVersion = 2019;

            registrationData = new RegistrationData(false, DateTime.MinValue);

            registrationTrack = Track.GetTrack(0);

        }

        public Season(int id, int racesCount, int finishedRaces, Track registrationTrack, List<Race> races = null, RegistrationData registrationData = null)
        {
            this.id = id;
            this.racesCount = racesCount;
            this.finishedRaces = finishedRaces;

            if (races == null)
            {
                this.races = new List<Race>(racesCount);
            }
            else
            {
                this.races = races;
            }

            if (registrationData == null)
            {
                registrationData = new RegistrationData(false, DateTime.MinValue);
            }

            this.registrationData = registrationData;

            this.registrationTrack = registrationTrack;
        }
    }
}
