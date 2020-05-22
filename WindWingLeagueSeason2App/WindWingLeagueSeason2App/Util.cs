using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace WindWingLeagueSeason2App
{
    public class Debug
    {
        public static void Log(object data)
        {
            File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOG.txt"), data.ToString() + "\n");
        }
    }
}
