using ProjektOrdner.App;
using ProjektOrdner.Forms;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Repository
{
    public class RepositoryRoot
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        public string RootPath { get; private set; }

        private AppSettings AppSettings { get; set; }

        private BackgroundWorker Worker { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public RepositoryRoot(string rootPath, AppSettings appSettings)
        {
            RootPath = rootPath;
            AppSettings = appSettings;
            Worker = new BackgroundWorker();
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 


        /// <summary>
        /// 
        /// Erstellt einen neuen RootOrdner.
        /// 
        /// </summary>
        public async Task CreateAsync(IProgress<string> progress)
        {
            progress.Report("Creating root folder...");
            CreateFolders();
            await CreateFilesAsync();
            await SetFolderAccessPermissionsAsync();

            progress.Report("Rootfolder successfully created!");
        }


        /// <summary>
        /// 
        /// Entfernt einen RootOrdner.
        /// 
        /// </summary>
        public async Task RemoveAsync(bool includeProjectsCleanup, IProgress<string> progress)
        {
            if (Directory.Exists(RootPath) == false)
                return;

            if (includeProjectsCleanup == true)
            {
                RepositoryFolder[] repositories = await GetRepositoriesAsync(true, progress);
                Array.ForEach(repositories, repository =>
                {
                    progress.Report($"Removing Projekt {repository.Organization.ProjektName}");
                    repository.Remove(progress);
                });
            }

            progress.Report($"Deleting all Files in Root...");
            Directory.Delete(RootPath, true);

            progress.Report($"Deleting completet!");
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public async Task<RepositoryFolder> GetRepositoryAsync(string folderName)
        {
            string folderPath = Path.Combine(RootPath, folderName);

            RepositoryFolder repository = new RepositoryFolder(AppSettings);
            await repository.Load(folderPath);

            return repository;
        }


        /// <summary>
        /// 
        /// Listet die vorhanden Projekte auf.
        /// 
        /// </summary>
        public DirectoryInfo[] GetRepositories()
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(RootPath);
            if (rootDirectory.Exists == false)
                return null;

            DirectoryInfo[] projektList = null;
            try
            {
                projektList = rootDirectory
                    .GetDirectories()
                    ?.Where(directory => directory.Name.StartsWith("_") == false)
                    ?.ToArray();
            }
            catch (Exception)
            {
                return null;
            }

            return projektList;
        }


        /// <summary>
        /// 
        /// Listet die vorhanden Projekte auf.
        /// 
        /// </summary>
        public async Task<RepositoryFolder[]> GetRepositoriesAsync(bool includeCorrupted, IProgress<string> progress)
        {
            progress.Report("Lade Projektliste...");
            string[] folderList = null;

            await Task.Run(() =>
            {
                folderList = GetRepositories()
                    ?.Where(folder =>
                        folder.Name.StartsWith("_") == false && // Unterstrich ausblenden
                        folder.Name.StartsWith(".") == false)  // Punkt ausblenden
                    ?.Select(directory => directory.FullName)
                    ?.ToArray();
            });


            if (null == folderList)
                return null;

            int i = 0;
            RepositoryProcessor repositoryProcessor = new RepositoryProcessor(new RepositoryRoot(RootPath, AppSettings), AppSettings, progress);
            List<RepositoryFolder> repositories = new List<RepositoryFolder>();
            List<Task<RepositoryFolder>> getRepoTasks = new List<Task<RepositoryFolder>>();

            await Task.Run(() =>
            {
                foreach (string folderPath in folderList)
                {
                    i++;
                    progress.Report($"Lade Projekt {i.ToString()}/{folderList.Count().ToString()} - {new DirectoryInfo(folderPath).Name}");
                    getRepoTasks.Add(repositoryProcessor.Get(folderPath));
                }
            });

            await Task.WhenAll(getRepoTasks);
            repositories.AddRange(getRepoTasks.Select(asdf => asdf.Result));

            // Exclude empty Projects
            repositories = repositories
                .Where(projekt => null != projekt)
                .ToList();

            // Exclude corrupted projects
            if (includeCorrupted == false)
                repositories = repositories
                    .Where(projekt => projekt.Status == RepositoryFolder.RepositoryStatus.Ok)
                    ?.ToList();

            return repositories.ToArray();
        }


        /// <summary>
        /// 
        /// Ruft die Anträge eines Repositorys ab.
        /// 
        /// </summary>
        public async Task<RepositoryOrganization[]> GetRepositoryRequestsAsync()
        {
            // Check PickupFolder
            string requestFolderPath = Path.Combine(RootPath, AppConstants.RequestFolderName);

            if (Directory.Exists(requestFolderPath) == false)
                return null;

            // Get Files
            string[] requestFiles = Directory.GetFiles(requestFolderPath);
            if (null == requestFiles)
                return null;

            // Filter Files
            IEnumerable<FileInfo> validRequestFiles = requestFiles
                    .Select(file => new FileInfo(file))
                    .Where(file => file.Name.StartsWith("_") == false)
                    .Where(file => file.Extension == ".txt");

            if (null == validRequestFiles || validRequestFiles.Count() == 0)
                return null;


            // Process Files
            List<RepositoryOrganization> requests = new List<RepositoryOrganization>();
            foreach (FileInfo file in validRequestFiles)
            {
                RepositoryOrganization organization = new RepositoryOrganization();
                await organization.LoadRequest(file.FullName);

                if (organization.IsValid() == true)
                    requests.Add(organization);
            }

            return requests.ToArray();
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public static async Task StartRootAssistantAsync(AppSettings appSettings, IProgress<string> progress)
        {
            RootFolderAssistentForm rootFolderAssistent = new RootFolderAssistentForm();
            DialogResult result = rootFolderAssistent.ShowDialog();

            if (result == DialogResult.Cancel)
                return;

            switch (rootFolderAssistent.RadioResult)
            {
                case 0:
                {
                    await appSettings.ManageRootPathsAsync();
                    break;
                }

                case 1:
                {
                    RepositoryRoot repositoryRoot = new RepositoryRoot(rootFolderAssistent.SelectedPath, appSettings);
                    await repositoryRoot.CreateAsync(progress);

                    if (appSettings.RootPaths.Contains(rootFolderAssistent.SelectedPath) == true)
                        return;

                    appSettings.RootPaths.Add(rootFolderAssistent.SelectedPath);
                    await appSettings.SaveAsync();

                    break;
                }

                default:
                    break;
            }

            MessageBox.Show("Die Änderung wurde erfolgreich übernommen!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 

        /// <summary>
        /// 
        /// Erstellt das Ordnerkonstrukt
        /// 
        /// </summary>
        private void CreateFolders()
        {
            if (Directory.Exists(RootPath) == false)
                Directory.CreateDirectory(RootPath);

            string requestFolderPath = Path.Combine(RootPath, AppConstants.RequestFolderName);
            if (Directory.Exists(requestFolderPath) == false)
                Directory.CreateDirectory(requestFolderPath);
        }


        /// <summary>
        /// 
        /// Erstellt und speichert eine Antragsdatei.
        /// 
        /// </summary>
        private async Task CreateFilesAsync()
        {
            string orgaFilePath = Path.Combine(RootPath, Path.Combine(AppConstants.RequestFolderName, "_Beispiel Antrag.txt"));
            await RepositoryOrganization.SaveRequestTemplateAsync(orgaFilePath, RepositoryOrganization.GetRequestTemplate());
        }


        /// <summary>
        /// 
        /// Setzt die Berchtigungen für den Rootordner.
        /// 
        /// </summary>
        private async Task SetFolderAccessPermissionsAsync(GroupPrincipal ownEntryGroup = null)
        {
            if (Directory.Exists(RootPath) == false)
                throw new DirectoryNotFoundException(RootPath);


            // SYSTEM - Access Rule - Full Control
            SecurityIdentifier systemSID = new SecurityIdentifier("S-1-5-18");
            FileSystemAccessRule folderAccessRule_System =
                new FileSystemAccessRule(
                    systemSID,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

            // local Administrator - Access Rule - Full Control
            SecurityIdentifier localAdminsSID = new SecurityIdentifier("S-1-5-32-544");
            FileSystemAccessRule folderAccessRule_localAdmin =
                new FileSystemAccessRule(
                    localAdminsSID,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

            // Domain Users - Access Rule - Only Access Folder
            string domainSID = ActiveDirectoryUtil.GetDomainSID().ToString();
            SecurityIdentifier domainUsersSID = new SecurityIdentifier($"{domainSID}-513");
            FileSystemAccessRule folderAccessRule_domainUsers =
                new FileSystemAccessRule(
                    domainUsersSID,
                    FileSystemRights.ReadAndExecute,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow);

            // Special permission DL Group - Access Rule - Only Access Folder
            //FileSystemAccessRule folderAccessRule_ownEntryGroup =
            //    new FileSystemAccessRule(
            //        ownEntryGroup.Sid.ToString(),
            //        FileSystemRights.Read,
            //        InheritanceFlags.ContainerInherit,
            //        PropagationFlags.NoPropagateInherit,
            //        AccessControlType.Allow);


            int i = 0;
            bool exitDo = false;
            DirectorySecurity directorySecurity = Directory.GetAccessControl(RootPath);

            do
            {
                i++; // Exit Counter

                // Exit afert 15 trys to get the SID
                if (i > 15)
                    throw new Exception("Could not get Group-SID from Active Directory!");

                try
                {
                    directorySecurity.AddAccessRule(folderAccessRule_System);
                    directorySecurity.AddAccessRule(folderAccessRule_localAdmin);
                    directorySecurity.AddAccessRule(folderAccessRule_domainUsers);
                    //directorySecurity.AddAccessRule(folderAccessRule_ownEntryGroup);

                    exitDo = true; // Exit OK
                }
                catch (IdentityNotMappedException)
                {
                    if (i <= 15)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 5));
                    }
                }
            } while (exitDo == false);

            // Apply AccessRules
            Directory.SetAccessControl(RootPath, directorySecurity);
        }


    }
}
