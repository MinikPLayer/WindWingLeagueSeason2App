using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WindWingLeagueSeason2App.Models
{
    public class User
    {
        public int id;
        public string login;
        public string steam;

        public bool good = true;

        public void LoadDefaults()
        {
            this.id = 0;
            this.login = "";
            this.steam = "";
        }

        public User()
        {

        }

        public User(string serialized)
        {
            LoadDefaults();

            good = Deserialize(serialized);
        }

        public User(int id, string login, string steam)
        {
            FillVariables(id, login, steam);
        }

        public void FillVariables(int id, string login, string steam)
        {
            this.id = id;
            this.login = login;
            this.steam = steam;
        }

        bool ParseSinglePacket(string header, string content)
        {
            try
            {
                switch (header)
                {
                    case "id":
                        this.id = int.Parse(content);
                        return true;

                    case "login":
                        this.login = content;
                        return true;

                    case "steam":
                        this.steam = content;
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

        public bool Deserialize(string str)
        {
            try
            {
                if (!str.StartsWith("user{"))
                {
                    Debug.LogError("[Season.SeasonUser.Deserialize] It's no a race packet, bad magic");
                    return false;
                }

                str = str.Substring(0, str.Length - 1).Remove(0, 5); // remove race{ and }

                List<string> packets = MUtil.SplitWithBrackets(str);
                for (int i = 0; i < packets.Count; i++)
                {
                    bool done = false;
                    for (int j = 0; j < packets[i].Length; j++)
                    {
                        if (packets[i][j] == '{')
                        {
                            if (!ParseSinglePacket(packets[i].Substring(0, j), packets[i].Substring(0, packets[i].Length - 1).Remove(0, j + 1)))
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
            return "user{id{" + id.ToString() + "},login{" + login + "},steam{" + steam + "}}";
        }

        public static async Task<User> GetUser(int id)
        {
            for(int i = 0;i<users.Count;i++)
            {
                if(users[i].id == id)
                {
                    return users[i];
                }
            }

            string response = await Views.MainPage.networkData.RequestAsync("Info;UD;" + id.ToString());
            string[] data = response.Split(';');
            if(data[0] == "OK")
            {
                User u = new User(data[1]);
                if(!u.good)
                {
                    return null;
                }
                users.Add(u);
                return u;
            }
            else
            {
                await Views.MainPage.singleton.DisplayAlert("Błąd", "Nie udało się pobrać danych użytkownika, spróbój ponownie później", "OK");

                return null;
            }
        }

        public static List<User> users = new List<User>();

    }
}
