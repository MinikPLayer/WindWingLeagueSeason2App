using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Xamarin.Forms;
using System.Security.Cryptography;

namespace WindWingLeagueSeason2App
{
    public class Debug
    {
        public static void Log(object data)
        {
            System.Diagnostics.Debug.WriteLine(data.ToString());
            if (Device.RuntimePlatform == Device.Android)
            {
                File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt"), data.ToString() + "\n");
            }

        }


    }

    public class MUtil
    {
        public static string HashSHA256(string data, string salt = "")
        {
            if (data == null) return "";
            using (SHA256 sha = SHA256.Create())
            {
                data += salt;
                return Encoding.Unicode.GetString(sha.ComputeHash(Encoding.Unicode.GetBytes(data)));
            }
        }

        public static string GetFilesPath(string additionalPath = "")
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), additionalPath);
        }

        public static bool FileExists(string fileName)
        {
            return File.Exists(GetFilesPath(fileName));
        }

        public static string ReadAllText(string fileName)
        {
            return File.ReadAllText(GetFilesPath(fileName));
        }

        public static void WriteAllText(string fileName, string content)
        {
            File.WriteAllText(GetFilesPath(fileName), content);
        }
    }
}
