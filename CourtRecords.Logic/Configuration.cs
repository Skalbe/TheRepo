using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourtRecords.Logic
{
    public class Config
    {
        public static string RecordStorageFolder { get; private set; }
        public static string PrintJobFolder { get; private set; }        
        public static string InputCacheFolder { get; private set; }

        static Config()
        {
            LoadConfig();
            
        }

        private static void LoadConfig()
        {
            RecordStorageFolder = ConfigurationSettings.AppSettings["RecordStorageFolder"];
            PrintJobFolder = ConfigurationSettings.AppSettings["PrintJobFolder"];
            InputCacheFolder = ConfigurationSettings.AppSettings["InputCacheFolder"];
        }
    }
}
