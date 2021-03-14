using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public class RepositoryProcessor
    {
        private AppSettingsModel AppSettings { get; set; }
        private ActiveDirectoryUtil ActiveDirectory { get; set; }

        public RepositoryProcessor(AppSettingsModel appSettings)
        {
            AppSettings = appSettings;
            ActiveDirectory = new ActiveDirectoryUtil(appSettings);
        }

        /// <summary>
        /// 
        /// Ruft einen ProjektOrdner ab
        /// 
        /// </summary>
        public async Task<RepositoryModel> GetRepositoryAsync(string folderPath, IProgress<string> progressMessage)
        {
            //progressMessage.Report($"Lade Projekt: {folderPath}");
            await Task.Delay(10);

            // Read RepositoryInfoFile
            RepositoryOrgaProcessor organisationFile = new RepositoryOrgaProcessor(folderPath);
            Task<RepositoryOrgaModel> organisationTask = organisationFile.GetInformationAsync();

            // Read ProjektSettings
            RepositorySettings repositorySettings = new RepositorySettings();
            await repositorySettings.Load(folderPath);


            RepositoryOrgaModel organisationResult = await organisationTask;

            // Check Values!!
            if (null == organisationResult ||
                string.IsNullOrWhiteSpace(organisationResult.ProjektName) == true ||
                (null == repositorySettings && organisationResult.Version == RepositoryVersion.V2))
            {
                // Konnte die Organisationsdatei nicht lesen oder verarbeiten!

                DirectoryInfo projektDirectory = new DirectoryInfo(folderPath);
                return new RepositoryModel
                {
                    RepositoryOrga = new RepositoryOrgaModel()
                    {
                        ProjektName = projektDirectory.Name,
                        RootPath = projektDirectory.Parent.FullName
                    },
                    Settings = new RepositorySettings(),
                    Version = RepositoryVersion.Unknown,
                    Status = RepositoryModel.RepositoryStatus.Corrupted
                };
            }

            // Setup Object
            RepositoryModel repository = new RepositoryModel
            {
                RepositoryOrga = organisationResult,
                Settings = repositorySettings,
                Version = organisationResult.Version,
                Status = RepositoryModel.RepositoryStatus.Ok
            };

            return repository;
        }


        /// <summary>
        /// 
        /// Ruft alle vorhanden ProjektOrdner ab
        /// 
        /// </summary>
        public async Task<RepositoryModel[]> GetRepositorysAsync(string rootPath, IProgress<string> progressMessage, bool includeCorrupted = false)
        {
            // Read every Folder
            await Task.Delay(10);

            // Get Directories
            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            DirectoryInfo[] directories = directoryInfo
                .GetDirectories()
                .Where(folder =>
                    folder.Name.StartsWith("_") == false && // Unterstrich ausblenden
                    folder.Name.StartsWith(".") == false)  // Punkt ausblenden
                .ToArray();

            // Get Projects from Directories
            progressMessage.Report("Lade Projekte ...");
            List<RepositoryModel> repositories = new List<RepositoryModel>();
            foreach (DirectoryInfo directory in directories)
            {
                // Process every Project
                repositories.Add(
                    await GetRepositoryAsync(directory.FullName, progressMessage));
            }

            // Exclude corrupted projects
            if (includeCorrupted == false)
                repositories = repositories
                    .Where(projekt => projekt.Status == RepositoryModel.RepositoryStatus.Ok)
                    ?.ToList();

            return repositories
                .Where(projekt => null != projekt)
                ?.ToArray();  // Exlude Null
        }


        /// <summary>
        /// 
        /// Fügt einen ProjektOrdner hinzu
        /// 
        /// </summary>
        public async Task AddRepositoryAsync(RepositoryModel repository, IProgress<string> progressMessage)
        {
            // Create Folder
            progressMessage.Report("Creating Folder... (1/4)");
            CreateFolders(repository.RepositoryOrga.ProjektPath);

            // Create Files
            progressMessage.Report("Creating Files... (2/4)");
            await CreateFilesAsync(repository.RepositoryOrga);

            // Create adn link Active Directory Groups
            progressMessage.Report("Creating Active Directory Groups... (3/4)");
            GroupPrincipal[] adGroups = CreateAdGroups(repository.RepositoryOrga.ProjektName);

            // Set Permission to Folder
            progressMessage.Report("Set up Directory ACLs (4/4)");
            SetFolderPermissions(repository.RepositoryOrga.ProjektPath, adGroups);
        }

        /// <summary>
        /// 
        /// Erstellt den Dateiordner
        /// 
        /// </summary>
        private void CreateFolders(string projektPath)
        {
            if (Directory.Exists(projektPath) == true)
                throw new Exception("Der ProjektOrdner existiert bereits!");

            // Creating Projektfolder
            Directory.CreateDirectory(projektPath);

            // Creating Organisationfolder
            string organisationFolderPath = Path.Combine(projektPath, AppConstants.OrganisationFolderName);
            Directory.CreateDirectory(organisationFolderPath);
        }


        private string GetPermissionFileTemplate(PermissionAccessRole permissionAccess)
        {
            string accessType = "";
            switch (permissionAccess)
            {
                case PermissionAccessRole.ReadOnly:
                {
                    accessType = "Nur Lesen";
                    break;
                }
                case PermissionAccessRole.ReadWrite:
                {
                    accessType = "Lesen & Schreiben";
                    break;
                }
                case PermissionAccessRole.Manager:
                {
                    accessType = "Manager";
                    break;
                }
            }

            string content = $@"
# # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
# 
# ProjektOrdner-Berechtigungsdatei - {accessType}
# 
# <> WICHTIG <>
# o Verwenden Sie nur den Windows-Anmeldename, Matrikelnummer oder die Emailadresse, um den Benutzer zu berechtigen.
# o Berechtigungen werden automatisiert halbstündlich :15 - :45 übernommen und Sie via Mail benachrichtigt.
# o Verändern Sie den Namen der Datei nicht.
##
# <> ANLEITUNGEN <>
# Die Anleitungen sind unter '_ProjektOrdner beantragen' zu finden.
#
# <> BEISPIELE <>
# stmustera
# 12345678
# profmuster
# max.mustermann@hs-kempten.de
#
";
            return content;
        }

        private async Task CreateFilesAsync(RepositoryOrgaModel organisation)
        {
            PermissionProcessor permissionProcessor = new PermissionProcessor(organisation.ProjektPath, AppSettings);
            List<Task> tasks = new List<Task>();

            // Create ReadOnly permission file
            string readOnlyFilePath = permissionProcessor.GetPermissionFilePath(PermissionAccessRole.ReadOnly, organisation.ProjektPath);
            using (StreamWriter writer = new StreamWriter(readOnlyFilePath, false, Encoding.UTF8))
            {
                tasks.Add(writer.WriteAsync(
                    GetPermissionFileTemplate(PermissionAccessRole.ReadOnly)));
            }

            // Create ReadWrite permission file
            string readWriteFilePath = permissionProcessor.GetPermissionFilePath(PermissionAccessRole.ReadWrite, organisation.ProjektPath);
            using (StreamWriter writer = new StreamWriter(readWriteFilePath, false, Encoding.UTF8))
            {
                tasks.Add(writer.WriteAsync(
                    GetPermissionFileTemplate(PermissionAccessRole.ReadWrite)));
            }

            // Create Manager permission file
            string managerFilePath = permissionProcessor.GetPermissionFilePath(PermissionAccessRole.Manager, organisation.ProjektPath);
            using (StreamWriter writer = new StreamWriter(managerFilePath, false, Encoding.UTF8))
            {
                tasks.Add(writer.WriteAsync(
                    GetPermissionFileTemplate(PermissionAccessRole.Manager)));
            }

            // Create Organisaion File
            RepositoryOrgaProcessor organisationFileProcessor = new RepositoryOrgaProcessor(organisation.ProjektPath);
            tasks.Add(organisationFileProcessor.WriteOrganisationAsync(organisation));

            // Create Settings File
            RepositorySettings repositorySettings = new RepositorySettings();
            tasks.Add(repositorySettings.Save(organisation.ProjektPath));



            await Task.WhenAll(tasks.ToArray());
        }

        private GroupPrincipal[] CreateAdGroups(string projektName)
        {
            string description = $"ProjektOrdner {projektName}; Angelegt am {DateTime.Now};";

            // Create local Groups
            GroupPrincipal[] adLocalGroups =
            {
                ActiveDirectory.AddGroup(GroupScope.Local, GetAdGroupName(projektName, GroupScope.Local, PermissionAccessRole.Manager), description),
                ActiveDirectory.AddGroup(GroupScope.Local, GetAdGroupName(projektName, GroupScope.Local, PermissionAccessRole.ReadWrite), description),
                ActiveDirectory.AddGroup(GroupScope.Local, GetAdGroupName(projektName, GroupScope.Local, PermissionAccessRole.ReadOnly), description),
            };

            // Create global Groups
            GroupPrincipal[] adGlobalGroups =
            {
                ActiveDirectory.AddGroup(GroupScope.Global, GetAdGroupName(projektName, GroupScope.Global, PermissionAccessRole.Manager), description),
                ActiveDirectory.AddGroup(GroupScope.Global, GetAdGroupName(projektName, GroupScope.Global, PermissionAccessRole.ReadWrite), description),
                ActiveDirectory.AddGroup(GroupScope.Global, GetAdGroupName(projektName, GroupScope.Global, PermissionAccessRole.ReadOnly), description)
            };

            // Link local and global Groups
            for (int i = 0; i <= 2; i++)
            {
                adLocalGroups[i].Members.Add(adGlobalGroups[i]);
                adLocalGroups[i].Save();
            }

            // Add Manager-Group to ReadWrite-Group
            adLocalGroups[1].Members.Add(adGlobalGroups[0]);
            adLocalGroups[1].Save();

            return adLocalGroups;
        }

        private void SetFolderPermissions(string projektPath, GroupPrincipal[] adLocalGroups)
        {
            if (null == adLocalGroups)
                throw new ArgumentNullException();

            // Variables
            string organisationFolderPath = Path.Combine(projektPath, AppConstants.OrganisationFolderName);


            // Filter Groups
            GroupPrincipal groupManager = adLocalGroups
                .Where(group => group.Name.EndsWith(AppSettings.AdGroupNameSuffixManager))
                .FirstOrDefault();

            GroupPrincipal groupReadWrite = adLocalGroups
                .Where(group => group.Name.EndsWith(AppSettings.AdGroupNameSuffixWrite))
                .FirstOrDefault();

            GroupPrincipal groupReadOnly = adLocalGroups
                .Where(group => group.Name.EndsWith(AppSettings.AdGroupNameSuffixRead))
                .FirstOrDefault();


            //
            // Projekt Root Folder - Definitions
            //

            // Access Rule ProjektRoot - Role ReadOnly - Read only inheritance!
            FileSystemAccessRule accessRuleReadOnlyWithInherit =
                new FileSystemAccessRule(
                    groupReadOnly.Sid,
                    FileSystemRights.ReadAndExecute,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

            // Access Rule ProjektRoot - Role ReadWrite - Read on Top only!
            FileSystemAccessRule accessRuleReadWriteNoInherit =
                new FileSystemAccessRule(
                    groupReadWrite.Sid,
                    FileSystemRights.ReadAndExecute | FileSystemRights.Write,
                    InheritanceFlags.ObjectInherit,
                    PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow);

            // Access Rule ProjektRoot - Role ReadWrite - Modify to sub folder and files inheritance!
            FileSystemAccessRule accessRuleReadWriteWithInherit =
                new FileSystemAccessRule(
                     groupReadWrite.Sid,
                    FileSystemRights.Modify,
                     InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                     PropagationFlags.InheritOnly,
                    AccessControlType.Allow);


            //
            // Organisation Folder - Definitions
            //

            // Access Rule ProjektRoot - Role Manager - Read on Top only!
            FileSystemAccessRule accessRuleMangerNoInherit =
                new FileSystemAccessRule(
                    groupManager.Sid,
                    FileSystemRights.ReadAndExecute | FileSystemRights.Write,
                    InheritanceFlags.ObjectInherit,
                    PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow);

            // Access Rule ProjektRoot - Role Manager - Modify to sub folder and files inheritance!
            FileSystemAccessRule accessRuleManagerWithInherit =
                new FileSystemAccessRule(
                     groupManager.Sid,
                    FileSystemRights.Modify,
                     InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                     PropagationFlags.InheritOnly,
                    AccessControlType.Allow);



            // Get AccessRules
            DirectorySecurity folderSecurtiy = Directory.GetAccessControl(projektPath);
            DirectorySecurity organisationSecurtiy = Directory.GetAccessControl(organisationFolderPath);

            // Stop Inheritance
            organisationSecurtiy.SetAccessRuleProtection(isProtected: true, preserveInheritance: true);

            // Update Control Lists
            int i = 0;
            bool exitDo = false;

            do
            {
                i++;

                try
                {
                    // Root Only
                    folderSecurtiy.AddAccessRule(accessRuleReadOnlyWithInherit);
                    folderSecurtiy.AddAccessRule(accessRuleReadWriteNoInherit);
                    folderSecurtiy.AddAccessRule(accessRuleReadWriteWithInherit);

                    // Organisation Folder
                    organisationSecurtiy.AddAccessRule(accessRuleMangerNoInherit);
                    organisationSecurtiy.AddAccessRule(accessRuleManagerWithInherit);

                    exitDo = true;
                }
                catch (IdentityNotMappedException)
                {
                    if (i >= 10)
                    {
                        exitDo = true;
                    }
                    else
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                }

            } while (exitDo == false);

            // Invoke Rules

            Directory.SetAccessControl(organisationFolderPath, organisationSecurtiy);
            Directory.SetAccessControl(projektPath, folderSecurtiy);
        }


        /// <summary>
        /// 
        /// Benennt den Projektordner um.
        /// 
        /// </summary>

        public async Task RenameRepositoryAsync(RepositoryModel repository, string newName, IProgress<string> progressMessage)
        {
            progressMessage.Report("Project sucessfully renamed!");
            await Task.Delay(1);

            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// Entfernt einen ProjektOrdner
        /// 
        /// </summary>
        public void RemoveRepository(RepositoryModel repository, IProgress<string> progressMessage)
        {
            // Remove all Files / Folders
            progressMessage.Report("Deleting projekt data... (1/3)");
            DeleteProjektData(repository.RepositoryOrga.ProjektPath);

            // Remove RepositoryInfoFile
            progressMessage.Report("Deleting repository data... (2/3)");
            DeleteRepositoryData(repository.RepositoryOrga.ProjektPath);

            // Remove Ad Groups
            progressMessage.Report("Deleting Active Directory Groups... (3/3)");
            DeleteAdGroups(repository.RepositoryOrga.ProjektName);

            progressMessage.Report("Project sucessfully deleted!");
        }

        private void DeleteRepositoryData(string projektPath)
        {
            DirectoryInfo projekt = new DirectoryInfo(projektPath);

            foreach (FileInfo file in projekt.GetFiles("*", SearchOption.AllDirectories))
            {
                file.Delete();
            }

            foreach (DirectoryInfo directory in projekt.GetDirectories("*", SearchOption.AllDirectories))
            {
                directory.Delete(true);
            }

            projekt.Delete();
        }

        private void DeleteProjektData(string projektPath)
        {
            DirectoryInfo projekt = new DirectoryInfo(projektPath);

            foreach (FileInfo file in projekt.GetFiles("*", SearchOption.AllDirectories))
            {
                if (file.Directory.Name == AppConstants.OrganisationFolderName)
                    continue;

                file.Delete();
            }

            foreach (DirectoryInfo directory in projekt.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (directory.Name == AppConstants.OrganisationFolderName)
                    continue;

                directory.Delete(true);
            }
        }

        private void DeleteAdGroups(string projektName)
        {
            foreach (GroupPrincipal group in GetAdGroups(projektName))
            {
                if (null != group)
                {
                    group.Delete();
                }
            }
        }


        //
        // Helper Functions
        //

        private List<GroupPrincipal> GetAdGroups(string projektName)
        {
            List<GroupPrincipal> groupList = new List<GroupPrincipal>();

            foreach (string groupNameItem in GetAdGroupNames(projektName))
            {
                GroupPrincipal group = ActiveDirectory.GetGroup(groupNameItem, IdentityType.SamAccountName);

                if (group != null)
                {
                    groupList.Add(group);
                }
            }

            return groupList;
        }

        private string GetAdGroupName(string projektName, GroupScope groupScope, PermissionAccessRole PermissionAccessRole)
        {
            string groupScopeName = "";
            switch (groupScope)
            {
                case GroupScope.Local:
                    groupScopeName = AppSettings.AdGroupScopeLocalName;
                    break;

                case GroupScope.Global:
                    groupScopeName = AppSettings.AdGroupScopeGlobalName;
                    break;
            }

            string adGroupSuffix = "";
            switch (PermissionAccessRole)
            {
                case PermissionAccessRole.Manager:
                {
                    adGroupSuffix = AppSettings.AdGroupNameSuffixManager;
                    break;
                }
                case PermissionAccessRole.ReadOnly:
                {
                    adGroupSuffix = AppSettings.AdGroupNameSuffixRead;
                    break;
                }
                case PermissionAccessRole.ReadWrite:
                {
                    adGroupSuffix = AppSettings.AdGroupNameSuffixWrite;
                    break;
                }
            }

            string projektNameWithoutSpaces = projektName.Replace(" ", "");
            string adGroupName = $@"{AppSettings.AdGroupNamePrefix}{groupScopeName}{AppSettings.AdGroupNameTopic}{projektNameWithoutSpaces}{adGroupSuffix}";

            return adGroupName.ToUpper();
        }

        private List<string> GetAdGroupNames(string projektName)
        {
            return new List<string>
            {
                GetAdGroupName(projektName, GroupScope.Local, PermissionAccessRole.ReadOnly),
                GetAdGroupName(projektName, GroupScope.Local, PermissionAccessRole.ReadWrite),
                GetAdGroupName(projektName, GroupScope.Local, PermissionAccessRole.Manager),
                GetAdGroupName(projektName, GroupScope.Global, PermissionAccessRole.ReadOnly),
                GetAdGroupName(projektName, GroupScope.Global, PermissionAccessRole.ReadWrite),
                GetAdGroupName(projektName, GroupScope.Global, PermissionAccessRole.Manager)
            };
        }

    }
}
