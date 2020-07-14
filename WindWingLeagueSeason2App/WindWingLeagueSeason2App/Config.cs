using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using WindWingLeagueSeason2App.Views;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App
{
    [Serializable]
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
                if (Device.RuntimePlatform == Device.UWP)
                {
                    Config conf = new Config();

                    conf.autoLogin = (bool)Application.Current.Properties["autologin"];
                    conf.autoLoginLogin = Application.Current.Properties["login"].ToString();
                    conf.autoLoginToken = Application.Current.Properties["token"].ToString();
                    return conf;
                }

                if (!File.Exists(MUtil.GetFilesPath(location)))
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
            try
            {
                if (config == null)
                {
                    config = MainPage.config;
                }

                if (Device.RuntimePlatform == Device.UWP)
                {
                    Application.Current.Properties["autologin"] = config.autoLogin;
                    Application.Current.Properties["login"] = config.autoLoginLogin;
                    Application.Current.Properties["token"] = config.autoLoginToken;
                    Application.Current.SavePropertiesAsync();
                    return;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                TextWriter writer = new StreamWriter(MUtil.GetFilesPath(location));
                serializer.Serialize(writer, config);
                writer.Close();
            }
            catch (Exception e)
            {
                Debug.Log("[Config.Save] Exception: " + e.ToString());
                return;
            }
        }
    }
}
