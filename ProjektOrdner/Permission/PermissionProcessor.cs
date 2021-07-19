using ProjektOrdner.App;
using ProjektOrdner.Repository;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public class PermissionProcessor
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        private AppSettings AppSettings { get; set; }
        private ActiveDirectoryUtil AdUtil { get; set; }
        private string ProjektPath { get; set; }
        private RepositoryVersion Version = RepositoryVersion.Unknown;


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public PermissionProcessor(RepositoryFolder repository, AppSettings appSettings)
        {
            ProjektPath = repository.Organization.ProjektPath;
            AppSettings = appSettings;
            Version = repository.Organization.Version;

            AdUtil = new ActiveDirectoryUtil(appSettings);
        }

        public PermissionProcessor(string projektPath, AppSettings appSettings)
        {
            ProjektPath = projektPath;
            AppSettings = appSettings;

            AdUtil = new ActiveDirectoryUtil(appSettings);
            DetectRepositoryVersion();
        }

        public PermissionProcessor(string projektPath, AppSettings appSettings, RepositoryVersion version)
        {
            ProjektPath = projektPath;
            AppSettings = appSettings;
            Version = version;

            AdUtil = new ActiveDirectoryUtil(appSettings);
        }

        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 


        /// <summary>
        /// 
        /// Gibt die Repository Berechtigungen aus
        /// 
        /// </summary>
        public async Task<RepositoryPermission[]> GetPermissionsAsync()
        {
            return await GetPermissionsFromFileAsync();
        }


        /// <summary>
        /// 
        /// Aktualisiert die Gruppenmitgliedschaften im Active Directory
        /// 
        /// </summary>
        public async Task SyncPermissionsAsync(RepositoryPermission[] newPermissions = null)
        {
            // Get Permissions

            RepositoryPermission[] permissionsFromFile;
            if (null != newPermissions)
            {
                permissionsFromFile = newPermissions;
            }
            else
            {
                permissionsFromFile = await GetPermissionsFromFileAsync();
            }
            RepositoryPermission[] permissionsFromAD = GetPermissionsFromAD();

            // Compare Permissions

            List<RepositoryPermission> permissionsToAdd = permissionsFromFile
                .Except(permissionsFromAD, new PermissionComparer())
                .ToList();

            List<RepositoryPermission> permissionsToRemove = permissionsFromAD
                .Except(permissionsFromFile, new PermissionComparer())
                .ToList();

            RepositoryOrganization repositoryOrganization = new RepositoryOrganization(ProjektPath);
            await repositoryOrganization.LoadOrganization();


            // Update Permissions

            if (null != permissionsToAdd && permissionsToAdd.Count > 0)
                await GrantPermissionsAsync(permissionsToAdd.ToArray());

            if (null != permissionsToRemove && permissionsToRemove.Count > 0)
                await RevokePermissionsAsync(permissionsToRemove.ToArray());
        }


        /// <summary>
        /// 
        /// Berechtigt die Berechtigungen auf das Projekt
        /// 
        /// </summary>
        public async Task GrantPermissionsAsync(RepositoryPermission[] permissions)
        {
            await Task.Run(async () =>
            {
                var groupedPermissions = permissions.GroupBy(permission => permission.Role);
                foreach (IGrouping<PermissionRole, RepositoryPermission> permissionGroup in groupedPermissions)
                {
                    if (permissionGroup.Count() == 0)
                        continue;

                    // Get File Content
                    string filePath = GetPermissionFilePath(permissionGroup.Key);
                    string fileContent = await ReadFileContentAsync(filePath);

                    // Get AD Data
                    string projektName = new DirectoryInfo(ProjektPath).Name;
                    string adGroupName = AdUtil.GetAdGroupName(projektName, GroupScope.Global, permissionGroup.Key);
                    GroupPrincipal adGroup = AdUtil.GetGroup(adGroupName, IdentityType.SamAccountName);

                    // Add to File & AD
                    foreach (var userPermission in permissionGroup)
                    {
                        // File
                        if (fileContent.Contains(userPermission.User.SamAccountName) == false)
                        {
                            fileContent = fileContent + Environment.NewLine + userPermission.User.SamAccountName;
                        }

                        // AD
                        UserPrincipal adUser = AdUtil.GetUser(userPermission.User.SamAccountName, IdentityType.SamAccountName);
                        AdUtil.AddGroupMembers(adGroup, adUser);
                    }

                    // Write File Content
                    await WriteFileContent(filePath, fileContent);
                }
            });
        }


        /// <summary>
        /// 
        /// Entfernt die Berechtigung auf das Projekt
        /// 
        /// </summary>
        public async Task RevokePermissionsAsync(RepositoryPermission[] permissions)
        {
            await Task.Run(async () =>
            {
                var groupedPermissions = permissions.GroupBy(permission => permission.Role);
                foreach (IGrouping<PermissionRole, RepositoryPermission> permissionGroup in groupedPermissions)
                {
                    if (permissionGroup.Count() == 0)
                        continue;

                    // Get File Content
                    string filePath = GetPermissionFilePath(permissionGroup.Key);
                    string fileContent = await ReadFileContentAsync(filePath);

                    // Get AD Data
                    string projektName = new DirectoryInfo(ProjektPath).Name;
                    string adGroupName = AdUtil.GetAdGroupName(projektName, GroupScope.Global, permissionGroup.Key);
                    GroupPrincipal adGroup = AdUtil.GetGroup(adGroupName, IdentityType.SamAccountName);

                    // Remove from File & AD
                    foreach (var userPermission in permissionGroup)
                    {
                        // File
                        if (fileContent.Contains(userPermission.User.SamAccountName) == true)
                        {
                            fileContent = fileContent.Replace(userPermission.User.SamAccountName, "");
                        }

                        // AD
                        UserPrincipal adUser = AdUtil.GetUser(userPermission.User.SamAccountName, IdentityType.SamAccountName);
                        AdUtil.RemoveGroupMember(adGroup, adUser);
                    }

                    // Write File Content
                    await WriteFileContent(filePath, fileContent);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static string GetPermissionFileName(PermissionRole role, RepositoryVersion version)
        {
            string fileName = "";
            switch (version)
            {
                case RepositoryVersion.V1:
                {
                    switch (role)
                    {
                        case PermissionRole.Guest:
                        {
                            fileName = AppConstants.PERMISSION_GUEST_FILE_NAME;
                            break;
                        }
                        case PermissionRole.Member:
                        {
                            fileName = AppConstants.PERMISSION_MEMBER_FILE_NAME;
                            break;
                        }
                        case PermissionRole.Manager:
                        {
                            fileName = AppConstants.PERMISSION_MANAGER_FILE_NAME;
                            break;
                        }
                        case PermissionRole.Undefined:
                        {
                            break;
                        }
                    }
                    break;
                }
                case RepositoryVersion.V2:
                {
                    switch (role)
                    {
                        case PermissionRole.Guest:
                        {
                            fileName = AppConstants.PERMISSION_GUEST_FILE_NAME;
                            break;
                        }
                        case PermissionRole.Member:
                        {
                            fileName = AppConstants.PERMISSION_MEMBER_FILE_NAME;
                            break;
                        }
                        case PermissionRole.Manager:
                        {
                            fileName = AppConstants.PERMISSION_MANAGER_FILE_NAME;
                            break;
                        }
                        case PermissionRole.Undefined:
                        {
                            break;
                        }
                    }
                    break;
                }
                case RepositoryVersion.Unknown:
                    break;
            }

            return fileName;
        }


        /// <summary>
        /// 
        /// </summary>
        public string GetPermissionFilePath(PermissionRole permissionAccess)
        {
            string fileName = string.Empty;
            switch (permissionAccess)
            {
                case PermissionRole.Guest:
                {
                    if (Version == RepositoryVersion.V1)
                    {
                        fileName = AppConstants.PERMISSION_READ_FILE_NAME;
                    }
                    else if (Version == RepositoryVersion.V2)
                    {
                        fileName = AppConstants.PERMISSION_GUEST_FILE_NAME;
                    }
                    break;
                }
                case PermissionRole.Member:
                {
                    if (Version == RepositoryVersion.V1)
                    {
                        fileName = AppConstants.PERMISSION_CHANGE_FILE_NAME;
                    }
                    else if (Version == RepositoryVersion.V2)
                    {
                        fileName = AppConstants.PERMISSION_MEMBER_FILE_NAME;
                    }
                    break;
                }
                case PermissionRole.Manager:
                {
                    if (Version == RepositoryVersion.V1)
                    {
                        fileName = AppConstants.OrganisationFileNameV1;
                    }
                    else if (Version == RepositoryVersion.V2)
                    {
                        fileName = AppConstants.PERMISSION_MANAGER_FILE_NAME;
                    }
                    break;
                }
            }

            string organisationPath = string.Empty;
            switch (Version)
            {
                case RepositoryVersion.V1:
                {
                    organisationPath = ProjektPath;
                    break;
                }
                case RepositoryVersion.V2:
                {
                    organisationPath = Path.Combine(ProjektPath, AppConstants.OrganisationFolderName);
                    break;
                }
                case RepositoryVersion.Unknown:
                    break;
            }

            // FilePath
            return Path.Combine(organisationPath, fileName);
        }


        /// <summary>
        /// 
        /// </summary>
        public static string GetPermissionFilePath(PermissionRole permissionAccess, string projektPath, RepositoryVersion version)
        {
            string fileName = string.Empty;
            switch (permissionAccess)
            {
                case PermissionRole.Guest:
                {
                    if (version == RepositoryVersion.V1)
                    {
                        fileName = AppConstants.PERMISSION_READ_FILE_NAME;
                    }
                    else if (version == RepositoryVersion.V2)
                    {
                        fileName = AppConstants.PERMISSION_GUEST_FILE_NAME;
                    }
                    break;
                }
                case PermissionRole.Member:
                {
                    if (version == RepositoryVersion.V1)
                    {
                        fileName = AppConstants.PERMISSION_CHANGE_FILE_NAME;
                    }
                    else if (version == RepositoryVersion.V2)
                    {
                        fileName = AppConstants.PERMISSION_MEMBER_FILE_NAME;
                    }
                    break;
                }
                case PermissionRole.Manager:
                {
                    if (version == RepositoryVersion.V1)
                    {
                        fileName = AppConstants.OrganisationFileNameV1;
                    }
                    else if (version == RepositoryVersion.V2)
                    {
                        fileName = AppConstants.PERMISSION_MANAGER_FILE_NAME;
                    }
                    break;
                }
            }


            string organisationPath = string.Empty;
            switch (version)
            {
                case RepositoryVersion.V1:
                {
                    organisationPath = projektPath;
                    break;
                }
                case RepositoryVersion.V2:
                {
                    organisationPath = Path.Combine(projektPath, AppConstants.OrganisationFolderName);
                    break;
                }
                case RepositoryVersion.Unknown:
                    break;
            }

            // FilePath
            return Path.Combine(organisationPath, fileName);
        }



        // // // // // // // // // // // // // // // // // // // // //
        // File IO
        // 


        /// <summary>
        /// 
        /// </summary>
        public void OpenPermissionFile(PermissionRole role)
        {
            string filePath = GetPermissionFilePath(role);

            // Start Programm
            Process process = new Process();
            process.StartInfo.FileName = filePath;
            process.Start();
        }


        /// <summary>
        /// 
        /// Ruft die Berechtigungen in den Berechtigungsdateien des Projektes ab.
        /// 
        /// </summary>
        private async Task<RepositoryPermission[]> GetPermissionsFromFileAsync()
        {
            List<RepositoryPermission> permissions = new List<RepositoryPermission>();

            await Task.Run(async () =>
            {
                foreach (PermissionRole role in Enum.GetValues(typeof(PermissionRole)))
                {
                    string permissionFilePath = GetPermissionFilePath(role);
                    if (File.Exists(permissionFilePath) == false)
                        continue;

                    if (role == PermissionRole.Manager)
                    {
                        if (Version == RepositoryVersion.V1)
                        {
                            // Extract ProjektManager Users
                            string managerFileContent = await ReadFileContentAsync(permissionFilePath);
                            string filteredLine = managerFileContent
                                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(line => line.ToLower().Contains("projektmanager=") == true)
                                .FirstOrDefault();

                            if (string.IsNullOrWhiteSpace(filteredLine) == true)
                                return;

                            string[] managerUsers = filteredLine
                                .Substring(filteredLine.IndexOf("=") + 1)
                                .Split(',');

                            // Create Objects
                            foreach (string manager in managerUsers)
                            {
                                RepositoryPermission permission = new RepositoryPermission(
                                    new AdUser(manager), PermissionRole.Manager, PermissionSource.File, AppSettings);

                                await permission.User.UpdateUserData();

                                if (permission.User.IsValidated == true)
                                    permissions.Add(permission);
                            }

                            continue;
                        }
                    }

                    string permissionFileContent = await ReadFileContentAsync(permissionFilePath);

                    // Filter Content
                    string[] users = permissionFileContent
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(line => line.Contains('#') == false && line.StartsWith("_") == false)
                        .Select(line => line.ToLower())
                        .ToArray();

                    // Create Permissions
                    foreach (string user in users)
                    {
                        RepositoryPermission permission = new RepositoryPermission(
                            new AdUser(user), role, PermissionSource.File, AppSettings);

                        await permission.User.UpdateUserData(); // Update Datea from Active Directory

                        if (permission.User.IsValidated == true)
                            permissions.Add(permission);
                    }
                }
            });

            return permissions.ToArray();
        }


        /// <summary>
        /// 
        /// Ließt den Dateiinhalt
        /// 
        /// </summary>
        private async Task<string> ReadFileContentAsync(string filePath)
        {
            string fileContent = string.Empty;
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            return fileContent;
        }


        /// <summary>
        /// 
        /// Schreibt in die Datei
        /// 
        /// </summary>
        private async Task WriteFileContent(string filePath, string fileContent)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(fileContent);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void DetectRepositoryVersion()
        {
            // Detect Repository Version
            RepositoryOrganization organization = new RepositoryOrganization(ProjektPath);
            RepositoryVersion repositoryVersion = organization.GetRepositoryVersion();

            Version = repositoryVersion;
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Validations
        // 


        /// <summary>
        /// 
        /// </summary>
        public bool FilesExist()
        {
            // Manager
            string managerFilePath = GetPermissionFilePath(PermissionRole.Manager);
            if (File.Exists(managerFilePath) == false)
                return false;

            // Member
            string memberFilePath = GetPermissionFilePath(PermissionRole.Member);
            if (File.Exists(memberFilePath) == false)
                return false;

            // Guest
            string guestFilePath = GetPermissionFilePath(PermissionRole.Guest);
            if (File.Exists(guestFilePath) == false)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool FilesValid()
        {
            throw new NotImplementedException();
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Active Directory IO
        // 

        /// <summary>
        /// 
        /// Ruft die Berechtigungen auf Basis der AD Gruppenmitgliedschaften ab
        /// 
        /// </summary>
        private RepositoryPermission[] GetPermissionsFromAD()
        {
            string projektName = new DirectoryInfo(ProjektPath).Name;
            List<RepositoryPermission> permissions = new List<RepositoryPermission>();

            string managerAdGroupName = AdUtil.GetAdGroupName(projektName, GroupScope.Global, PermissionRole.Manager);
            string memberAdGroupName = AdUtil.GetAdGroupName(projektName, GroupScope.Global, PermissionRole.Member);
            string guestAdGroupName = AdUtil.GetAdGroupName(projektName, GroupScope.Global, PermissionRole.Guest);

            PrincipalCollection managerUsers = AdUtil.GetGroupMembers(managerAdGroupName, IdentityType.SamAccountName);
            PrincipalCollection memberUsers = AdUtil.GetGroupMembers(memberAdGroupName, IdentityType.SamAccountName);
            PrincipalCollection guestUsers = AdUtil.GetGroupMembers(guestAdGroupName, IdentityType.SamAccountName);

            foreach (UserPrincipal manager in managerUsers)
                permissions.Add(
                    new RepositoryPermission(
                        new AdUser(manager),
                        PermissionRole.Manager,
                        PermissionSource.ActiveDirectory,
                        AppSettings));

            foreach (UserPrincipal member in memberUsers)
                permissions.Add(
                    new RepositoryPermission(
                        new AdUser(member),
                        PermissionRole.Member,
                        PermissionSource.ActiveDirectory,
                        AppSettings));

            foreach (UserPrincipal guest in guestUsers)
                permissions.Add(
                    new RepositoryPermission(
                        new AdUser(guest),
                        PermissionRole.Guest,
                        PermissionSource.ActiveDirectory,
                        AppSettings));

            return permissions.ToArray();
        }
    }
}
