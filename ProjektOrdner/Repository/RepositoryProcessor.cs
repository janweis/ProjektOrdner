using MimeKit;
using ProjektOrdner.App;
using ProjektOrdner.Mail;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Variables
        //

        private RepositoryRoot Root { get; set; }
        private AppSettings AppSettings { get; set; }
        private IProgress<string> Progress { get; set; }
        private ActiveDirectoryUtil DirectoryUtils { get; set; }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Constructors
        //

        public RepositoryProcessor(RepositoryRoot root, AppSettings appSettings)
            : this(root, appSettings, new Progress<string>())
        { }

        public RepositoryProcessor(RepositoryRoot root, AppSettings appSettings, IProgress<string> progress)
        {
            Root = root;
            AppSettings = appSettings;
            Progress = progress;

            DirectoryUtils = new ActiveDirectoryUtil(appSettings);
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Get Repository
        //

        /// <summary>
        /// 
        /// Ruft einen ProjektOrdner ab
        /// 
        /// </summary>
        /// 
        public async Task<RepositoryFolder> Get(string folderPath)
        {
            // Read RepositoryInfoFile
            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(folderPath);
            await repositoryOrganization.LoadOrganization();

            // Read ProjektSettings
            RepositorySettings repositorySettings = new RepositorySettings();
            await repositorySettings.Load(folderPath);

            // Validation
            RepositoryFolder.RepositoryStatus repositoryStatus = RepositoryFolder.RepositoryStatus.Ok;
            if (repositoryOrganization.IsValid() == false)
                repositoryStatus = RepositoryFolder.RepositoryStatus.Corrupted;

            return new RepositoryFolder(
                repositoryOrganization,
                repositorySettings,
                RepositoryVersion.V2,
                AppSettings)
            { Status = repositoryStatus };
        }



        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Create Repository
        //

        /// <summary>
        /// 
        /// Create ProjektOrdner repository
        /// 
        /// </summary>
        public async Task CreateAsync(RepositoryOrganization organization)
            => await CreateAsync(organization, false);

        public async Task CreateAsync(RepositoryOrganization organization, bool IsMigration)
            => await CreateAsync(organization, IsMigration, null);

        public async Task CreateAsync(RepositoryOrganization organization, bool IsMigration, RepositorySettings settings)
        {
            await Task.Run(async () =>
            {
                if (null == organization)
                    return;

                // Create Folder
                Progress.Report("(1/5) Creating Folder...");
                CreateFolders(organization.ProjektPath, IsMigration);

                // Create Files
                Progress.Report("(2/5) Creating Files...");
                await CreateFilesAsync(organization, settings);

                // Create adn link Active Directory Groups
                Progress.Report("(3/5) Creating Active Directory Groups...");
                GroupPrincipal[] adGroups = await CreateAdGroups(organization);

                // Set Permission to Folder
                Progress.Report("(4/5) Set up Directory ACLs");
                await SetProjektFolderPermissionsAsync(organization.ProjektPath, adGroups);

                // Inform Users via Mail
                Progress.Report("(5/5) Send Mailmessages to inform users");
                //SendMailmessages();
            });
        }


        /// <summary>
        /// 
        /// Erstellt den Dateiordner
        /// 
        /// </summary>
        private void CreateFolders(string projektPath, bool isMigration = false)
        {
            // Nur ausführen, wenn kein Ordner migriert wird!
            if (isMigration == false)
            {
                if (Directory.Exists(projektPath) == true)
                    throw new Exception("Der ProjektOrdner existiert bereits!");

                // Creating Projektfolder
                Directory.CreateDirectory(projektPath);
            }

            // Creating Organisationfolder
            string organisationFolderPath = Path.Combine(projektPath, AppConstants.OrganisationFolderName);
            Directory.CreateDirectory(organisationFolderPath);
        }


        /// <summary>
        /// 
        /// Erstellt die Repositorydateien
        /// 
        /// </summary>
        private async Task CreateFilesAsync(RepositoryOrganization organisation, RepositorySettings settings)
        {
            PermissionProcessor permissionProcessor = new PermissionProcessor(organisation.ProjektPath, AppSettings, RepositoryVersion.V2);

            // Get default settings, if empty
            if (null == settings)
                settings = new RepositorySettings();

            List<Task> fileCreationTasks = new List<Task>();

            // Create ReadOnly permission file
            string readOnlyFilePath = permissionProcessor.GetPermissionFilePath(PermissionRole.Guest);
            using (StreamWriter writer = new StreamWriter(readOnlyFilePath, false, Encoding.UTF8))
            {
                fileCreationTasks.Add(writer.WriteAsync(PermissionFileTemplate.GetPermissionTemplate(PermissionRole.Guest)));
            }

            // Create ReadWrite permission file
            string readWriteFilePath = permissionProcessor.GetPermissionFilePath(PermissionRole.Member);
            using (StreamWriter writer = new StreamWriter(readWriteFilePath, false, Encoding.UTF8))
            {
                fileCreationTasks.Add(writer.WriteAsync(PermissionFileTemplate.GetPermissionTemplate(PermissionRole.Member)));
            }

            // Create Manager permission file
            string managerFilePath = permissionProcessor.GetPermissionFilePath(PermissionRole.Manager);
            using (StreamWriter writer = new StreamWriter(managerFilePath, false, Encoding.UTF8))
            {
                fileCreationTasks.Add(writer.WriteAsync(PermissionFileTemplate.GetPermissionTemplate(PermissionRole.Manager)));
            }

            // Create Organisaion File
            fileCreationTasks.Add(organisation.Save(RepositoryVersion.V2));

            // Create Settings File
            fileCreationTasks.Add(settings.Save(organisation.ProjektPath));

            await Task.WhenAll(fileCreationTasks);
        }


        /// <summary>
        /// 
        /// Erstellt die Active Directory Gruppen
        /// 
        /// </summary>
        private async Task<GroupPrincipal[]> CreateAdGroups(RepositoryOrganization organization)
        {
            GroupPrincipal[] groups = null;

            await Task.Run(() =>
             {
                 string description = $"ProjektOrdner {organization.ProjektName}; Angelegt am {DateTime.Now};";
                 string notes = organization.ProjektPath;

                 // Create local Groups
                 GroupPrincipal[] adLocalGroups =
                 {
                    DirectoryUtils.NewAdGroup(GroupScope.Local, DirectoryUtils.GetAdGroupName(organization.ProjektName, GroupScope.Local, PermissionRole.Manager), description),
                    DirectoryUtils.NewAdGroup(GroupScope.Local, DirectoryUtils.GetAdGroupName(organization.ProjektName, GroupScope.Local, PermissionRole.Member), description),
                    DirectoryUtils.NewAdGroup(GroupScope.Local, DirectoryUtils.GetAdGroupName(organization.ProjektName, GroupScope.Local, PermissionRole.Guest), description),
                 };

                 // Create global Groups                        
                 GroupPrincipal[] adGlobalGroups =
                 {
                    DirectoryUtils.NewAdGroup(GroupScope.Global,DirectoryUtils.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Manager), description),
                    DirectoryUtils.NewAdGroup(GroupScope.Global,DirectoryUtils.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Member), description),
                    DirectoryUtils.NewAdGroup(GroupScope.Global,DirectoryUtils.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Guest), description)
                 };

                 // Link local and global Groups
                 for (int i = 0; i <= 2; i++)
                 {
                     adLocalGroups[i].Members.Add(adGlobalGroups[i]);
                     adLocalGroups[i].Save();
                 }

                 // Add Manager-Group to ReadWrite-Group
                 DirectoryUtils.AddGroupMembers(adLocalGroups[1], adGlobalGroups[0]);
                 groups = adLocalGroups;
             });

            return groups;
        }


        /// <summary>
        /// 
        /// Setzt die Berechtigungen für den ProjektOrdner.
        /// 
        /// </summary>
        private async Task SetProjektFolderPermissionsAsync(string projektPath, GroupPrincipal[] adLocalGroups)
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
                        await Task.Delay(new TimeSpan(0, 0, 5));
                        //Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                }

            } while (exitDo == false);

            // Invoke Rules

            Directory.SetAccessControl(organisationFolderPath, organisationSecurtiy);
            Directory.SetAccessControl(projektPath, folderSecurtiy);
        }


        /// <summary>
        /// 
        /// Informiert die Manager über die Projektanlage
        /// 
        /// </summary>
        private void SendMailmessages(RepositoryOrganization organization, RepositoryFolder repository)
        {
            string subject = "Ihr ProjektOrdner wurde angelegt!";
            MailProcessor mailProcessor = new MailProcessor(AppSettings, repository);

            // Create Mails
            List<MimeMessage> mails = new List<MimeMessage>();
            foreach (AdUser user in organization.LegacyPermissions
                .Where(permission => permission.Role == PermissionRole.Manager)
                .Select(permission => permission.User))
            {
                MailTemplateCreator templateCreator = new MailTemplateCreator(user, organization);
                string mailtext = templateCreator.ProjektCreatedTemplate();

                // Craete and add Mail to List
                mails.Add(mailProcessor.CreateMail(mailtext, subject, user));
            }

            // Send Mails
            foreach (MimeMessage mail in mails)
            {
                try
                {
                    mailProcessor.SendMail(mail);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }



        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Remove Repository
        //

        /// <summary>
        /// 
        /// Entfernt einen ProjektOrdner
        /// 
        /// </summary>
        public async Task RemoveAsync(string folderPath)
        {
            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(folderPath);
            await repositoryOrganization.LoadOrganization();

            await RemoveAsync(repositoryOrganization);
        }

        public async Task RemoveAsync(RepositoryOrganization organization)
        {
            // Remove all Files / Folders
            Progress.Report("Deleting projekt data... (1/3)");
            await DeleteProjektData(organization.ProjektPath);

            // Remove RepositoryInfoFile
            Progress.Report("Deleting repository data... (2/3)");
            await DeleteRepositoryData(organization.ProjektPath);

            // Remove Ad Groups
            Progress.Report("Deleting Active Directory Groups... (3/3)");
            await DeleteAdGroups(organization.ProjektName);

            Progress.Report("Project sucessfully deleted!");
        }


        /// <summary>
        /// 
        /// Löscht die Daten im Repository
        /// 
        /// </summary>
        private async Task DeleteRepositoryData(string folderPath)
        {
            await Task.Run(() =>
            {
                DirectoryInfo projekt = new DirectoryInfo(folderPath);
                foreach (FileInfo file in projekt.GetFiles("*", SearchOption.AllDirectories))
                {
                    file.Delete();
                }

                foreach (DirectoryInfo directory in projekt.GetDirectories("*", SearchOption.AllDirectories))
                {
                    directory.Delete(true);
                }

                projekt.Delete();
            });
        }


        /// <summary>
        /// 
        /// Löscht alle Repository Daten
        /// 
        /// </summary>
        private async Task DeleteProjektData(string folderPath)
        {
            await Task.Run(() =>
            {
                DirectoryInfo projekt = new DirectoryInfo(folderPath);
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
            });
        }


        /// <summary>
        /// 
        /// Löscht die AD Gruppen
        /// 
        /// </summary>
        private async Task DeleteAdGroups(string projektName)
        {
            await Task.Run(() =>
            {
                foreach (GroupPrincipal group in DirectoryUtils.GetAdGroups(projektName))
                {
                    if (null != group)
                    {
                        group.Delete();
                    }
                }
            });
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Rename Repository
        //

        public async Task RenameRepository(string repositoryName, string newName)
        {
            if (repositoryName == newName)
                return;

            string projektPath = Path.Combine(Root.RootPath, repositoryName);
            string newProjektPath = Path.Combine(Root.RootPath, newName);

            if (Directory.Exists(projektPath) == false)
                throw new DirectoryNotFoundException();

            // Set Orgafile
            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(projektPath);
            await repositoryOrganization.LoadOrganization();
            repositoryOrganization.ProjektName = newName;
            await repositoryOrganization.Save();

            // Rename Repository
            await Task.Run(() => { Directory.Move(projektPath, newProjektPath); });

            // Rename AdGroups
            List<string> adGroupNames = DirectoryUtils.GetAdGroupNames(repositoryName);
            List<string> adGroupNewNames = DirectoryUtils.GetAdGroupNames(newName);

            for (int i = 0; i < adGroupNames.Count; i++)
            {
                await DirectoryUtils.RenameGroupAsync(adGroupNames[i], adGroupNewNames[i]);
            }
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Repair Repository
        //

        public async Task RepairAsync(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) == true)
                throw new ArgumentNullException("folderPath");

            // Folders
            if (FoldersAreValid(folderPath) == false)
                CreateFolders(folderPath, true);

            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(folderPath) { ProjektEnde = DateTime.Now.AddMonths(6) };

            // Files
            if (FilesAreValid(folderPath) == false)
                await CreateFilesAsync(
                    repositoryOrganization,
                    new RepositorySettings());

            // Active Directory Groups
            if (ActiveDirectoryGroupsAreValid(repositoryOrganization.ProjektName) == false)
                await CreateAdGroups(repositoryOrganization);

            // Folder Permissions
            await SetProjektFolderPermissionsAsync(folderPath, DirectoryUtils.GetAdGroups(repositoryOrganization.ProjektName)
                .ToArray());
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // Validate Repository
        //


        /// <summary>
        /// 
        /// </summary>
        public bool FoldersAreValid(string folderPath)
        {
            if (Directory.Exists(folderPath) == false)
                return false;

            string orgaFolderPath = Path.Combine(folderPath, AppConstants.OrganisationFileNameV2);
            if (Directory.Exists(orgaFolderPath) == false)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool FilesAreValid(string folderPath)
        {
            string orgaFolderPath = Path.Combine(folderPath, AppConstants.OrganisationFileNameV2);

            PermissionProcessor permissionProcessor = new PermissionProcessor(folderPath, AppSettings);
            if (permissionProcessor.FilesExist() == false)
                return false;

            RepositorySettings repositorySettings = new RepositorySettings();
            if (repositorySettings.FileExist(folderPath) == false)
                return false;

            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(folderPath);
            if (repositoryOrganization.FileExist() == false)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool ActiveDirectoryGroupsAreValid(string projektName)
        {
            if (DirectoryUtils.GetAdGroups(projektName).Count != 6)
                return false;

            return true;
        }

    }
}
