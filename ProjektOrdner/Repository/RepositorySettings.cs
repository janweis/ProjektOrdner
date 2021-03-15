using Newtonsoft.Json;
using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public class RepositorySettings
    {
        //
        // Variablen
        //

        // Bei Berechtigungsänderungen werden die ProjektManager über die Änderung nach der Umsetzung informiert.
        // Standard: SendMailByPermissionReadWrites = 1 
        public int MailByPermissionReadWrites { get; set; } = 1;

        // Die Projektmitglieder werden informiert, wenn Sie dem Projekt beitreten
        // Standard: SendMailByJoining = 1
        public int MailByJoining { get; set; } = 1;

        // Die Projektmitglieder werden informiert, wenn Sie das Projekt verlassen
        // Standard: SendMailByLeaving = 1
        public int MailByLeaving { get; set; } = 1;

        // Über die restliche Laufzeit des ProjektOrdners informieren
        // Standard: SendMailByExpiring = 1
        public int MailByExpiring { get; set; } = 1;

        // Einzelne Mitglieder vom Mailemfpang ausnehmen
        // Standard: MailReceipientExclution = 
        public string[] MailRecipientExclution { get; set; } = new string[0];

        // ProjektOrdner Version
        public string SettingsVersion { get; set; } = "1.0.0.0";

        // Sonderfunktion
        public bool ProjektVorEndeDerLaufzeitEntfernen { get; set; } = false;
        public bool SindSieSichSicher { get; set; } = false;


        //
        // Constructors
        //

        public RepositorySettings()
        {
            LoadDefaults();
        }


        //
        // Functions
        // 


        /// <summary>
        /// 
        /// Läd die default Einstellungen
        /// 
        /// </summary>
        public void LoadDefaults()
        {
            MailByExpiring = 1;
            MailByJoining = 1;
            MailByLeaving = 1;
            MailByPermissionReadWrites = 1;
            MailRecipientExclution = null;
            ProjektVorEndeDerLaufzeitEntfernen = false;
            SindSieSichSicher = false;
        }


        /// <summary>
        /// 
        /// Schreibt die Einstellungen in die Einstellungsdatei des Projekts
        /// 
        /// </summary>
        public async Task<bool> Save(string folderPath)
        {
            string filePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.RepositorySettingsFileName));

            if (Directory.Exists(folderPath) == true)
            {
                string fileContent = JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
                using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await streamWriter.WriteAsync(fileContent);
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// Ließt die Einstellungen aus der Einstellungsdatei des Projekts
        /// 
        /// </summary>
        public async Task<bool> Load(string folderPath)
        {
            string filePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.OrganisationFileNameV2));

            if (File.Exists(filePath) == true)
            {
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    Task<string> fileContentTask = streamReader.ReadToEndAsync();
                    RepositorySettings projektSettings = JsonConvert.DeserializeObject<RepositorySettings>(
                        await fileContentTask,
                        new JsonSerializerSettings() { Formatting = Formatting.Indented });

                    if (null == projektSettings)
                        return false;

                    MailByExpiring = projektSettings.MailByExpiring;
                    MailByJoining = projektSettings.MailByJoining;
                    MailByLeaving = projektSettings.MailByLeaving;
                    MailByPermissionReadWrites = projektSettings.MailByPermissionReadWrites;
                    MailRecipientExclution = projektSettings.MailRecipientExclution;
                    ProjektVorEndeDerLaufzeitEntfernen = projektSettings.ProjektVorEndeDerLaufzeitEntfernen;
                    SindSieSichSicher = projektSettings.SindSieSichSicher;
                }

                return true;
            }

            return false;
        }

    }
}
