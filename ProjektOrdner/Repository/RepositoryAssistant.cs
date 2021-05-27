using ProjektOrdner.App;
using ProjektOrdner.Forms;
using ProjektOrdner.Permission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Repository
{
    public class RepositoryAssistant
    {
        public RepositoryRoot RootFolder { get; set; }
        private RepositoryFolder Processor { get; set; }
        private RepositoryProcessor RepositoryProcessor { get; set; }
        private AppSettings AppSettings { get; set; }
        private IProgress<string> Progress { get; set; }

        public RepositoryAssistant(string rootFolder, IProgress<string> progress, AppSettings appSettings)
        {
            RootFolder = new RepositoryRoot(rootFolder, appSettings);
            AppSettings = appSettings;
            Progress = progress;

            Processor = new RepositoryFolder(appSettings);
            RepositoryProcessor = new RepositoryProcessor(new RepositoryRoot(rootFolder, appSettings), appSettings, progress);
        }


        /// <summary>
        /// 
        /// Erstellt ein neues Projekt.
        /// 
        /// </summary>
        public async Task<bool> CreateRepositoryAsync()
        {
            // Collect Projekt Data
            GetRepositoryNameForm getDataForm = new GetRepositoryNameForm();
            getDataForm.ShowDialog();

            if (getDataForm.DialogResult == DialogResult.Cancel)
            {
                Progress.Report("Project creation cancelled!");
                return false;
            }

            // Create Repository with Data
            string projektName = getDataForm.ProjektName;
            DateTime projektEnde = getDataForm.ProjektEnde;
            RepositoryOrganization organisation = new RepositoryOrganization(projektName, RootFolder.RootPath, projektEnde, RepositoryVersion.V2);

            // Add Repository
            await RepositoryProcessor.CreateAsync(organisation);

            // Start PermissionManager
            if (getDataForm.UsePermissionAssistent == true)
            {
                PermissionManager permissionManager = new PermissionManager(organisation.ProjektPath, AppSettings);
                await permissionManager.ManagePermissions();
            }

            return true;
        }


        /// <summary>
        /// 
        /// Benennt das Projekt um.
        /// 
        /// </summary>
        public async Task<bool> RenameRepositoryAsync(string folderPath)
        {
            // Get current directory name
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            string currentName = directory.Name;

            // Get new repository name
            RenameProjektForm renameProjekt = new RenameProjektForm(currentName);
            DialogResult result = renameProjekt.ShowDialog();

            if (result == DialogResult.Cancel)
            {
                Progress.Report("Project renaming cancelled!");
                return false;
            }

            string newName = renameProjekt.Name;

            // Get & Rename Repository
            RepositoryFolder repository = await RepositoryProcessor.Get(currentName);
            await Processor.RenameAsync(repository, newName, Progress);

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task MigrateFolderToRepositoryAsync()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath) == true)
                return;



            // Collect Projekt Data
            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(fbd.SelectedPath);
            GetRepositoryNameForm getDataForm = new GetRepositoryNameForm(repositoryOrganization.ProjektName);
            getDataForm.ShowDialog();

            if (getDataForm.DialogResult == DialogResult.Cancel)
            {
                Progress.Report("Project creation cancelled!");
                return;
            }

            // Create Repository with Data
            repositoryOrganization.ProjektEnde = getDataForm.ProjektEnde;
            await RepositoryProcessor.CreateAsync(repositoryOrganization, true);
        }
    }
}
