using Newtonsoft.Json;
using ProjektOrdner.Forms;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.App
{
    public class AppSettings
    {
        // // // // // // // // // // // // // // // // // // // // // 
        // Variablen - Common
        //

        private readonly string SettingsFilePath = Path.Combine(Environment.CurrentDirectory, AppConstants.AppSettingsFileName);


        // // // // // // // // // // // // // // // // // // // // // 
        // Variablen - Active Directory
        //

        // Active Directory Domänen Name
        public string AdDomainName { get; set; } = Environment.UserDomainName;

        // Dort werden die AD-BenutzerModel ausgelesen
        // Distinguished Name wird benötigt
        public string AdUserRootDN { get; set; }

        // Dort werden die GlobalGroup AD-Gruppen angelegt
        // Distinguished Name wird benötigt
        public string AdGroupGgDN { get; set; }

        // Dort werden die DomainLocalGroup AD-Gruppen angelegt
        // Distinguished Name wird benötigt
        public string AdGroupDlDN { get; set; }

        // Prefix 
        public string AdGroupNamePrefix { get; set; }

        // AD-Gruppen Bezeichnung für das Thema
        public string AdGroupNameTopic { get; set; }

        // Suffix für den lesenen Zugriff Gruppennamen
        public string AdGroupNameSuffixRead { get; set; }

        // Suffix für den schreibenden Zugriff Gruppennamen
        public string AdGroupNameSuffixWrite { get; set; }

        // Suffix für den manager Zugriff Gruppennamen
        public string AdGroupNameSuffixManager { get; set; }


        public string AdGroupScopeGlobalName { get; set; }
        public string AdGroupScopeLocalName { get; set; }


        // Fortlaufende GruppenID für künftige AD-Gruppen
        public int AdGroupID { get; set; } = 0;


        // Dort werden die Root-Ordner gespeichert
        public List<string> RootPaths { get; set; }


        public string RootPathDefault { get; set; }


        // Dort wird die Protokollierung eingeschaltet
        public bool Logging { get; set; } = false;

        // Protokollpfad
        public string LogPath { get; set; } = "";


        //
        // BenutzerModel limitierungen
        //

        public int UserMaxProjektOrdner { get; set; } = 0;
        public string UserDeniedCreateProjekts { get; set; }


        //
        // Mail System
        //

        // VON
        public string MailFrom { get; set; }

        // Ein- und Ausschalter für das senden von Info-Mails
        public int MailDisabled { get; set; } = 0;

        // Angabe des FDQN des Mailservers
        public string MailServer { get; set; }

        // Angabe des Mailports für das Versenden von Mails
        public int MailPort { get; set; } = 25;


        // // // // // // // // // // // // // // // // // // // // // 
        // Constructors
        //

        public AppSettings()
        {
            RootPaths = new List<string>();
        }


        // // // // // // // // // // // // // // // // // // // // // 
        // Functions
        //

        /// <summary>
        /// 
        /// Läd die Settings
        /// 
        /// </summary>
        public async Task<bool> LoadAsync()
        {
            if (File.Exists(SettingsFilePath) == true)
            {
                using (StreamReader streamReader = new StreamReader(SettingsFilePath, Encoding.UTF8))
                {
                    Task<string> fileContentTask = streamReader.ReadToEndAsync();
                    AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(
                        await fileContentTask,
                        new JsonSerializerSettings() { Formatting = Formatting.Indented });

                    if (null == appSettings)
                        return false;

                    // Load Settings...
                    AdDomainName = appSettings.AdDomainName;
                    AdUserRootDN = appSettings.AdUserRootDN;
                    AdGroupGgDN = appSettings.AdGroupGgDN;
                    AdGroupDlDN = appSettings.AdGroupDlDN;
                    AdGroupNamePrefix = appSettings.AdGroupNamePrefix;
                    AdGroupNameTopic = appSettings.AdGroupNameTopic;
                    AdGroupNameSuffixRead = appSettings.AdGroupNameSuffixRead;
                    AdGroupNameSuffixWrite = appSettings.AdGroupNameSuffixWrite;
                    AdGroupNameSuffixManager = appSettings.AdGroupNameSuffixManager;
                    AdGroupScopeGlobalName = appSettings.AdGroupScopeGlobalName;
                    AdGroupScopeLocalName = appSettings.AdGroupScopeLocalName;
                    AdGroupID = appSettings.AdGroupID;
                    RootPathDefault = appSettings.RootPathDefault;
                    Logging = appSettings.Logging;
                    LogPath = appSettings.LogPath;
                    UserMaxProjektOrdner = appSettings.UserMaxProjektOrdner;
                    UserDeniedCreateProjekts = appSettings.UserDeniedCreateProjekts;
                    MailFrom = appSettings.MailFrom;
                    MailDisabled = appSettings.MailDisabled;
                    MailServer = appSettings.MailServer;
                    MailPort = appSettings.MailPort;

                    if (null == appSettings.RootPaths)
                    {
                        RootPaths = new List<string>();
                    }
                    else
                    {
                        RootPaths = appSettings.RootPaths;
                    }
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// Speichert die Settings
        /// 
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            string fileContent = JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
            using (StreamWriter streamWriter = new StreamWriter(SettingsFilePath, false, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(fileContent);
            }

            return true;
        }


        /// <summary>
        /// 
        /// Bearbeitet die RootPaths
        /// 
        /// </summary>
        public async Task ManageRootPathsAsync()
        {
            // Show Form
            EditRootPathsForm manageRootPaths = new EditRootPathsForm(RootPaths);
            DialogResult dialogResult = manageRootPaths.ShowDialog();

            if (RootPaths.Count > 0)
                if (string.IsNullOrWhiteSpace(RootPathDefault) == true)
                    RootPathDefault = RootPaths.First();

            if (dialogResult == DialogResult.Cancel)
            {
                // Nichts zu erledigen, Abbruch!
                return;
            }
            else
            {
                if (dialogResult == DialogResult.OK)
                {
                    await SaveAsync();
                }
            }
        }

    }
}