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
    public class RepositoryManager
    {
        public string RootFolder { get; set; }
        RepositoryFolder Processor { get; set; }
        AppSettings AppSettings { get; set; }

        public RepositoryManager(string rootFolder, AppSettings appSettings)
        {
            RootFolder = rootFolder;
            AppSettings = appSettings;

            Processor = new RepositoryFolder(appSettings);
        }


        /// <summary>
        /// 
        /// Erstellt ein neues Projekt.
        /// 
        /// </summary>
        public async Task CreateRepositoryAsync(IProgress<string> messageProgress)
        {
            // Collect Projekt Data
            GetRepositoryNameForm getDataForm = new GetRepositoryNameForm();
            getDataForm.ShowDialog();

            if (getDataForm.DialogResult == DialogResult.Cancel)
            {
                messageProgress.Report("Project creation cancelled!");
                return;
            }

            // Create Repository with Data
            string projektName = getDataForm.ProjektName;
            DateTime projektEnde = getDataForm.ProjektEnde;

            RepositoryOrganization organisation = new RepositoryOrganization()
            {
                ErstelltAm = DateTime.Now,
                ProjektName = projektName,
                ProjektEnde = projektEnde,
                RootPath = RootFolder,
                Version = RepositoryVersion.V2
            };

            // Add Repository
            RepositoryFolder repository = new RepositoryFolder(organisation, new RepositorySettings(), RepositoryVersion.V2, AppSettings);
            await repository.CreateAsync(messageProgress);

            // Start PermissionManager
            if(getDataForm.UsePermissionAssistent == true)
            {
                PermissionManager permissionManager = new PermissionManager(organisation.ProjektPath, AppSettings);
                await permissionManager.ManagePermissions();
            }
        }


        /// <summary>
        /// 
        /// Benennt das Projekt um.
        /// 
        /// </summary>
        public async Task RenameRepositoryAsync(string folderPath, IProgress<string> messageProgress)
        {
            // Get current directory name
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            string currentName = directory.Name;
            
            // Get new repository name
            RenameProjektForm renameProjekt = new RenameProjektForm(currentName);
            DialogResult result = renameProjekt.ShowDialog();

            if (result == DialogResult.Cancel)
            {
                messageProgress.Report("Project renaming cancelled!");
                return;
            }

            string newName = renameProjekt.Name;

            // Get & Rename Repository
            RepositoryFolder repository = await Processor.Get(folderPath, messageProgress);
            await Processor.RenameAsync(repository, newName,messageProgress);
        }


        /// <summary>
        /// 
        /// Entfernt das Projekt.
        /// 
        /// </summary>
        public async Task RemoveRespositoryAsync(string folderPath, IProgress<string> messageProgress)
        {
            RepositoryFolder repositoryFolder = new RepositoryFolder(AppSettings);
            repositoryFolder = await repositoryFolder.Get(folderPath, messageProgress);
            repositoryFolder.Remove(messageProgress);
        }

    }
}
