using System;
using System.Collections.Generic;
using System.Text;

namespace WindWingLeagueSeason2App.Models
{
    public class User
    {
        public int id;
        public string login;
        public string steam;

        public User()
        {

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

    }
}
