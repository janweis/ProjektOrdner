using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public static class AppConstants
    {
        // File System Configuration
        public static string OrganisationFolderName = "_Organisation";

        public static string OrganisationFileNameV2 = "ProjektOrdner.json";
        public static string OrganisationFileNameV1 = "_ProjektInfo";
        public static string PermissionFileManagerName = "_Manager-Zugriff.txt";
        public static string PermissionFileReadWriteName = "_Mitarbeiter-Zugriff.txt";
        public static string PermissionFileReadOnlyName = "_Gast-Zugriff.txt";
        public static string RepositorySettingsFileName = "Einstellungen.json";
        public static string AppSettingsFileName = "AppSettings.json";
        public static string RequestFolderName = "_Projekt beantragen";

        // File System Log
        public static string LogFileName = "ErrorLog_ProjektOrdner.txt";
        public static string LogFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ProjektOrdner");
        public static string LogFilePath = Path.Combine(LogFolderPath, LogFileName);

        // Active Directory Configuration
        public static string AdGroupPrefix = "";
        public static string AdGroupReadOnlyNameSuffix = "RO";
        public static string AdGroupReadWriteNameSuffix = "RW";
        public static string AdGroupManagerNameSuffix = "PM";

        // Mail
        public static string MailFromAddress = "it-support@institut-ifm.de";
        public static string MailAdressPassword = "FcgKdZ3Dy4xpE-MvPAjJSk9wh";
        public static string MailServer = "IfmSrvEx01.institut-ifm.de";
        public static int MailPort = 587;
        public static bool MailUsingSSL = false;
    }
}
