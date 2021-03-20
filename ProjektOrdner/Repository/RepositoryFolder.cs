using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
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
    public class RepositoryFolder
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        public enum RepositoryStatus { Ok, Corrupted, NotChecked }

        public RepositoryOrganization Organization { get; set; }
        public RepositorySettings Settings { get; set; }
        public RepositoryVersion Version { get; set; }
        public RepositoryStatus Status { get; set; }

        private AppSettings AppSettings { get; set; }
        private ActiveDirectoryUtil ActiveDirectory { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public RepositoryFolder(AppSettings appSettings)
        {
            AppSettings = appSettings;
            ActiveDirectory = new ActiveDirectoryUtil(appSettings);

            Version = RepositoryVersion.Unknown;
            Status = RepositoryStatus.NotChecked;
        }

        public RepositoryFolder(RepositoryOrganization organization, RepositorySettings settings, RepositoryVersion version, AppSettings appSettings)
        {
            AppSettings = appSettings;
            ActiveDirectory = new ActiveDirectoryUtil(appSettings);

            Organization = organization;
            Settings = settings;
            Version = version;
            Status = RepositoryStatus.NotChecked;
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Main Functions
        // 

        /// <summary>
        /// 
        /// Ruft einen ProjektOrdner ab und läd ihn in dieses Objekt
        /// 
        /// </summary>
        public async Task<bool> Load(string folderPath)
        {
            try
            {
                // Read RepositoryInfoFile
                Organization = new RepositoryOrganization();
                await Organization.LoadV2(folderPath);

                // Read ProjektSettings
                Settings = new RepositorySettings();
                await Settings.Load(folderPath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// Ruft einen ProjektOrdner ab
        /// 
        /// </summary>
        public async Task<RepositoryFolder> Get(string folderPath, IProgress<string> progressMessage)
        {
            // Read RepositoryInfoFile
            RepositoryOrganization repositoryOrganization = new RepositoryOrganization();
            RepositoryVersion repositoryVersion = repositoryOrganization.GetRepositoryVersion(folderPath);

            if (repositoryVersion == RepositoryVersion.V2)
            {
                await repositoryOrganization.LoadV2(folderPath);
            }
            else
            {
                if (repositoryVersion == RepositoryVersion.V1)
                    await repositoryOrganization.LoadV1(folderPath);
            }

            // Read ProjektSettings
            RepositorySettings repositorySettings = new RepositorySettings();
            await repositorySettings.Load(folderPath);

            // Check Values!!
            if (repositoryOrganization.IsValid() == false)
            {
                // Konnte die Organisationsdatei nicht lesen oder verarbeiten!

                DirectoryInfo projektDirectory = new DirectoryInfo(folderPath);
                return new RepositoryFolder(AppSettings)
                {
                    Organization = new RepositoryOrganization()
                    {
                        ProjektName = projektDirectory.Name,
                        RootPath = projektDirectory.Parent.FullName
                    },
                    Settings = new RepositorySettings(),
                    Version = RepositoryVersion.Unknown,
                    Status = RepositoryFolder.RepositoryStatus.Corrupted
                };
            }

            // Setup Object
            RepositoryFolder repository = new RepositoryFolder(AppSettings)
            {
                Organization = repositoryOrganization,
                Settings = repositorySettings,
                Version = repositoryOrganization.Version,
                Status = RepositoryFolder.RepositoryStatus.Ok
            };

            return repository;
        }


        /// <summary>
        /// 
        /// Fügt einen ProjektOrdner hinzu
        /// 
        /// </summary>
        public async Task CreateAsync(RepositoryFolder repository, IProgress<string> progressMessage)
        {
            // Create Folder
            progressMessage.Report("Creating Folder... (1/4)");
            CreateFolders(repository.Organization.ProjektPath);

            // Create Files
            progressMessage.Report("Creating Files... (2/4)");
            await CreateFilesAsync(repository.Organization);

            // Create adn link Active Directory Groups
            progressMessage.Report("Creating Active Directory Groups... (3/4)");
            GroupPrincipal[] adGroups = CreateAdGroups(repository.Organization.ProjektName);

            // Set Permission to Folder
            progressMessage.Report("Set up Directory ACLs (4/4)");
            await SetProjektFolderPermissions(repository.Organization.ProjektPath, adGroups);
        }


        /// <summary>
        /// 
        /// Benennt den Projektordner um.
        /// 
        /// </summary>
        public async Task RenameAsync(RepositoryFolder repository, string newName, IProgress<string> progressMessage)
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
        public void Remove(IProgress<string> progressMessage)
        {
            // Remove all Files / Folders
            progressMessage.Report("Deleting projekt data... (1/3)");
            DeleteProjektData();

            // Remove RepositoryInfoFile
            progressMessage.Report("Deleting repository data... (2/3)");
            DeleteRepositoryData();

            // Remove Ad Groups
            progressMessage.Report("Deleting Active Directory Groups... (3/3)");
            DeleteAdGroups();

            progressMessage.Report("Project sucessfully deleted!");
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public void Verify() { throw new NotImplementedException(); }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public void Repair() { throw new NotImplementedException(); }





        // // // // // // // // // // // // // // // // // // // // //
        // Sub Functions
        // 


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


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private async Task CreateFilesAsync(RepositoryOrganization organisation)
        {
            PermissionProcessor permissionProcessor = new PermissionProcessor(organisation.ProjektPath, AppSettings);

            // Create ReadOnly permission file
            string readOnlyFilePath = permissionProcessor.GetPermissionFilePath(PermissionAccessRole.ReadOnly, organisation.ProjektPath);
            using (StreamWriter writer = new StreamWriter(readOnlyFilePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(PermissionProcessor.GetPermissionTemplate(PermissionAccessRole.ReadOnly));
            }

            // Create ReadWrite permission file
            string readWriteFilePath = permissionProcessor.GetPermissionFilePath(PermissionAccessRole.ReadWrite, organisation.ProjektPath);
            using (StreamWriter writer = new StreamWriter(readWriteFilePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(PermissionProcessor.GetPermissionTemplate(PermissionAccessRole.ReadWrite));
            }

            // Create Manager permission file
            string managerFilePath = permissionProcessor.GetPermissionFilePath(PermissionAccessRole.Manager, organisation.ProjektPath);
            using (StreamWriter writer = new StreamWriter(managerFilePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(PermissionProcessor.GetPermissionTemplate(PermissionAccessRole.Manager));
            }

            // Create Organisaion File
            await organisation.SaveV2();

            // Create Settings File
            RepositorySettings repositorySettings = new RepositorySettings();
            await repositorySettings.Save(organisation.ProjektPath);
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private GroupPrincipal[] CreateAdGroups(string Name)
        {
            string description = $"ProjektOrdner {Name}; Angelegt am {DateTime.Now};";

            // Create local Groups
            GroupPrincipal[] adLocalGroups =
            {
                ActiveDirectory.AddGroup(GroupScope.Local, ActiveDirectory.GetAdGroupName(Name, GroupScope.Local, PermissionAccessRole.Manager), description),
                ActiveDirectory.AddGroup(GroupScope.Local, ActiveDirectory.GetAdGroupName(Name, GroupScope.Local, PermissionAccessRole.ReadWrite), description),
                ActiveDirectory.AddGroup(GroupScope.Local, ActiveDirectory.GetAdGroupName(Name, GroupScope.Local, PermissionAccessRole.ReadOnly), description),
            };

            // Create global Groups                        
            GroupPrincipal[] adGlobalGroups =
            {
                ActiveDirectory.AddGroup(GroupScope.Global,ActiveDirectory.GetAdGroupName(Name, GroupScope.Global, PermissionAccessRole.Manager), description),
                ActiveDirectory.AddGroup(GroupScope.Global,ActiveDirectory.GetAdGroupName(Name, GroupScope.Global, PermissionAccessRole.ReadWrite), description),
                ActiveDirectory.AddGroup(GroupScope.Global,ActiveDirectory.GetAdGroupName(Name, GroupScope.Global, PermissionAccessRole.ReadOnly), description)
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


        /// <summary>
        /// 
        /// Setzt die Berechtigungen für den ProjektOrdner.
        /// 
        /// </summary>
        private async Task SetProjektFolderPermissions(string projektPath, GroupPrincipal[] adLocalGroups)
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





        private void DeleteRepositoryData()
        {
            DirectoryInfo projekt = new DirectoryInfo(Organization.ProjektPath);

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

        private void DeleteProjektData()
        {
            DirectoryInfo projekt = new DirectoryInfo(Organization.ProjektPath);

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

        private void DeleteAdGroups()
        {
            foreach (GroupPrincipal group in ActiveDirectory.GetAdGroups(Organization.ProjektName))
            {
                if (null != group)
                {
                    group.Delete();
                }
            }
        }


        public override string ToString()
        {
            return $@"\n
- Name: '{Organization.ProjektName}'
- Ende: {Organization.ProjektEnde.ToShortDateString()}
- Version: {Version.ToString()}
";
        }

    }
}
