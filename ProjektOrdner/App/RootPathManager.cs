using ProjektOrdner.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.App
{
    public class RootPathManager
    {
        public AppSettingsModel AppSettings { get; set; }

        public RootPathManager(AppSettingsModel appSettings)
        {
            AppSettings = appSettings;
        }


        /// <summary>
        /// 
        /// Startet den Manager zur Pfadverwaltung.
        /// 
        /// </summary>

        public async Task ManageAsync()
        {
            List<string> startPathList = GetPaths();

            // Show Form
            RootPathManageForm manageRootPaths = new RootPathManageForm(startPathList);
            DialogResult dialogResult = manageRootPaths.ShowDialog();

            if (dialogResult == DialogResult.Cancel)
            {
                // Nichts zu erledigen, Abbruch!
                return;
            }
            else
            {
                if (dialogResult == DialogResult.OK)
                {
                    await WritePaths(manageRootPaths.RootPaths);
                }
            }
        }


        /// <summary>
        /// 
        /// Ließt die Pfade aus.
        /// 
        /// </summary>

        private List<string> GetPaths()
        {
            if (null != AppSettings?.RootFolders?.RootProfiles)
            {
                return AppSettings.RootFolders.RootProfiles
                    .Select(folder => folder.Path)
                    .ToList();
            }

            throw new Exception("Could not get root path list!");
        }


        /// <summary>
        /// 
        /// Schreibt die Pfade in die Json-Datei.
        /// 
        /// </summary>

        private async Task WritePaths(List<string> pathList)
        {
            if (null == pathList)
                throw new ArgumentNullException();

            if (null == AppSettings.RootFolders)
            {
                AppSettings.RootFolders = new AppSettingsRootProfileProcessor();
            }

            AppSettings.RootFolders.Reset();

            foreach (string pathItem in pathList)
            {
                AppSettings.RootFolders.Add(pathItem);
            }

            // Write new Settings
            AppSettingsProcessor settingsProcessor = new AppSettingsProcessor();
            await settingsProcessor.WriteSettingsAsync(AppSettings);
        }
    }
}
