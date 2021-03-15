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
        RepositoryProcessor Processor { get; set; }
        AppSettings AppSettings { get; set; }

        public RepositoryManager(string rootFolder, AppSettings appSettings)
        {
            RootFolder = rootFolder;
            AppSettings = appSettings;

            Processor = new RepositoryProcessor(appSettings);
        }


        /// <summary>
        /// 
        /// Erstellt ein neues Projekt.
        /// 
        /// </summary>

        public async Task CreateRepositoryAsync(IProgress<string> messageProgress)
        {
            // Collect Projekt Data
            CreateRepositoryForm getDataForm = new CreateRepositoryForm();
            getDataForm.ShowDialog();

            if (getDataForm.DialogResult == DialogResult.Cancel)
            {
                messageProgress.Report("Project creation cancelled!");
                return;
            }

            // Create Repository with Data
            string Name = getDataForm.Name;
            DateTime projektEnde = getDataForm.ProjektEnde;

            RepositoryOrganization organisation = new RepositoryOrganization()
            {
                ErstelltAm = DateTime.Now,
                Name = Name,
                EndeDatum = projektEnde,
                RootPath = RootFolder,
                Version = RepositoryVersion.V2
            };

            RepositoryModel repository = new RepositoryModel(organisation, new RepositorySettings(), RepositoryVersion.V2);

            // Add Repository
            await Processor.AddRepositoryAsync(repository, messageProgress);

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
            RepositoryModel repository = await Processor.GetRepositoryAsync(folderPath, messageProgress);
            await Processor.RenameRepositoryAsync(repository, newName,messageProgress);
        }


        /// <summary>
        /// 
        /// Entfernt das Projekt.
        /// 
        /// </summary>

        public async Task RemoveRespositoryAsync(string folderPath, IProgress<string> messageProgress)
        {
            RepositoryModel repository = await Processor.GetRepositoryAsync(folderPath, messageProgress);
            Processor.RemoveRepository(repository, messageProgress);
        }

    }
}
