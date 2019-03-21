using Koenig.Maestro.Operation.Framework;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Koenig.Maestro.Operation
{
    public sealed class MaestroApplication
    {
        
        static string connectionString = string.Empty;
        static int cacheReloadSpan = 600;
        static MaestroApplication instance = null;
        static IConfigurationRoot configRoot;
        static string qbAppName = string.Empty;
        static string qbAppId = string.Empty;
        static string qbAppPath = string.Empty;
        static short qbMajorVersion = 0;
        static short qbMinorVersion = 0;
        static string qbCountryCode = string.Empty;
        
        public readonly string UNKNOWN_ITEM_NAME = "UNKNOWN";

        

        MaestroApplication()
        {
            
        }

        public static IConfigurationRoot ConfigRoot
        {
            get
            {
                return configRoot;
            }
            set
            {
                configRoot = value;
                connectionString = configRoot.GetConnectionString("MaestroConnection");
                int.TryParse(configRoot.GetSection("ApplicationSettings")["CacheExpire"], out cacheReloadSpan);
                qbAppId = configRoot.GetSection("QuickBooks")["ApplicationID"];
                qbAppName = configRoot.GetSection("QuickBooks")["ApplicationName"];
                qbAppPath = configRoot.GetSection("QuickBooks")["FilePath"];
                short.TryParse(configRoot.GetSection("QuickBooks")["MajorVersion"], out qbMajorVersion);
                short.TryParse(configRoot.GetSection("QuickBooks")["MinorVersion"], out qbMinorVersion);
                qbCountryCode = configRoot.GetSection("QuickBooks")["Country"];
            }
        }

        public static MaestroApplication Instance
        {
            get
            {
                if (instance == null)
                    instance = new MaestroApplication();

                return instance;
            }
        }

        public TimeSpan ReloadTimeSpan
        {
            get
            {
                return new TimeSpan(0, cacheReloadSpan, 0);
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            
        }

        public string QuickBooksAppId{get{ return qbAppId; }}
        public string QuickBooksAppName { get { return qbAppName; } }
        public string QuickBooksAppPath { get { return qbAppPath; } }

        public string QuickBooksCountry { get { return qbCountryCode; } }
        public short QuickBooksMajorVersion { get { return qbMajorVersion; } }
        public short QuickBooksMinorVersion { get { return qbMinorVersion; } }
    }


}
