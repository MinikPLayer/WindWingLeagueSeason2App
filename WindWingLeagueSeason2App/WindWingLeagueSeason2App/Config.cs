using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using WindWingLeagueSeason2App.Views;

namespace WindWingLeagueSeason2App
{
    [System.Serializable]
    public class Config
    {
        public bool autoLogin;
        public string autoLoginLogin;
        public string autoLoginToken;

        public bool darkMode;

        public static Config Load(string location = "config.xml")
        {
            try
            {
                if(!File.Exists(MUtil.GetFilesPath(location)))
                {
                    Debug.Log("[Config.Load] Config file doesn't exist");
                    return new Config();
                }

                XmlSerializer deserializer = new XmlSerializer(typeof(Config));
                return (Config)deserializer.Deserialize(new StreamReader(MUtil.GetFilesPath(location)));
            }
            catch(Exception e)
            {
                Debug.Log("[Config.Load] Exception: " + e.ToString());
                return new Config();
            }
        }

        public static void Save(Config config = null, string location = "config.xml")
        {
            if (config == null)
            {
                config = MainPage.config;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            TextWriter writer = new StreamWriter(MUtil.GetFilesPath(location));
            serializer.Serialize(writer, config);
            writer.Close();
        }
    }
}
