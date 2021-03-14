using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjektOrdner.App;

namespace ProjektOrdner.Repository
{
    public class RepositorySettingsProcessor
    {
        private string SettingsFilePath { get; set; }

        public RepositorySettingsProcessor(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException();

            SettingsFilePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, "Einstellungen.json"));
        }


        /// <summary>
        /// 
        /// Generiert die Standardeinstellungen.
        /// 
        /// </summary>

        public static RepositorySettingsModel GetDefaultSettings()
        {
            return new RepositorySettingsModel
            {
                MailByExpiring = 1,
                MailByJoining = 1,
                MailByLeaving = 1,
                MailByPermissionReadWrites = 1,
                MailRecipientExclution = null,
                ProjektVorEndeDerLaufzeitEntfernen = false,
                SindSieSichSicher = false
            };
        }


        /// <summary>
        /// 
        /// Schreibt die Einstellungen in die Einstellungsdatei des Projekts
        /// 
        /// </summary>

        public async Task WriteSettingsAsync(RepositorySettingsModel appSettings)
        {
            if (null == appSettings)
                throw new ArgumentNullException();

            // Serializing
            string fileContent = JsonConvert.SerializeObject(appSettings, new JsonSerializerSettings { Formatting = Formatting.Indented });

            // File writing...
            using (StreamWriter streamWriter = new StreamWriter(SettingsFilePath, false, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(fileContent);
            }
        }


        /// <summary>
        /// 
        /// Ließt die Einstellungen aus der Einstellungsdatei des Projekts
        /// 
        /// </summary>

        public async Task<RepositorySettingsModel> ReadSettingsAsync()
        {
            if (File.Exists(SettingsFilePath) == false)
                return null; // File not exists!

            // File reading...
            RepositorySettingsModel projektSettings;
            using (StreamReader streamReader = new StreamReader(SettingsFilePath, Encoding.UTF8))
            {
                Task<string> fileContentTask = streamReader.ReadToEndAsync();
                projektSettings = JsonConvert.DeserializeObject<RepositorySettingsModel>(
                    await fileContentTask,
                    new JsonSerializerSettings() { Formatting = Formatting.Indented });
            }

            return projektSettings;
        }
    }
}
