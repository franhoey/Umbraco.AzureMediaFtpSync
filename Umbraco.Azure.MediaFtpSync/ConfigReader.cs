using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Azure.MediaFtpSync
{
    class ConfigReader
    {
        public const string APP_SETTING_ENABLED = "Azure.MediaFtpSync.Enabled";
        public const string APP_SETTING_FTP_WEBSITE_ROOT = "Azure.MediaFtpSync.FtpWebsiteRoot";
        public const string APP_SETTING_FTP_DOMAIN = "Azure.MediaFtpSync.FtpDomain";
        public const string APP_SETTING_FTP_USERNAME = "Azure.MediaFtpSync.FtpUserName";
        public const string APP_SETTING_FTP_PASSWORD = "Azure.MediaFtpSync.FtpPassword";

        public string FtpWebsiteRoot { get; set; }
        public string FtpDomain { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public bool Enabled { get; set; }
        
        public static ConfigReader FromAppSettings()
        {
            return new ConfigReader()
            {
                Enabled = Convert.ToBoolean(ConfigurationManager.AppSettings[APP_SETTING_ENABLED]),
                FtpWebsiteRoot = ConfigurationManager.AppSettings[APP_SETTING_FTP_WEBSITE_ROOT],
                FtpDomain = ConfigurationManager.AppSettings[APP_SETTING_FTP_DOMAIN],
                FtpUserName = ConfigurationManager.AppSettings[APP_SETTING_FTP_USERNAME],
                FtpPassword = ConfigurationManager.AppSettings[APP_SETTING_FTP_PASSWORD]
            };
        }

    }
}
