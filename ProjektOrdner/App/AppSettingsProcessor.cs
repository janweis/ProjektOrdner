using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public class AppSettingsProcessor
    {
        private string SettingsFilePath { get; set; }

        public AppSettingsProcessor()
        {
            SettingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings.json");
        }

        /// <summary>
        /// 
        /// Schreibt den Textinhalt in die Datei
        /// 
        /// </summary>

        public async Task WriteSettingsAsync(AppSettingsModel appSettings)
        {
            if (null == appSettings)
                throw new ArgumentNullException();

            JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonKonfiguration = JsonConvert.SerializeObject(appSettings, jsonSerializerOptions);

            using (StreamWriter writer = new StreamWriter(SettingsFilePath, false, System.Text.Encoding.UTF8))
            {
                await writer.WriteAsync(jsonKonfiguration);
            }
        }


        /// <summary>
        /// 
        /// Ließt den Textinhalt aus einer Datei
        /// 
        /// </summary>

        public async Task<AppSettingsModel> ReadSettingsAsync()
        {
            JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string fileContent = string.Empty;
            using (StreamReader reader = new StreamReader(SettingsFilePath, System.Text.Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<AppSettingsModel>(fileContent, jsonSerializerOptions);
        }
    }
}
