using ProjektOrdner.App;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public class PermissionProcessor
    {
        private AppSettings AppSettings { get; set; }
        private ActiveDirectoryUtil AdUtil { get; set; }
        private string ProjektPath { get; set; }
        private RepositoryVersion OrdnerVersion { get; set; }

        public PermissionProcessor(string projektPath, AppSettings appSettings)
        {
            ProjektPath = projektPath;
            AppSettings = appSettings;
            OrdnerVersion = RepositoryVersion.V2;

            AdUtil = new ActiveDirectoryUtil(appSettings);
        }

        public PermissionProcessor(string projektPath, AppSettings appSettings, RepositoryVersion version)
        {
            ProjektPath = projektPath;
            AppSettings = appSettings;
            OrdnerVersion = version;

            AdUtil = new ActiveDirectoryUtil(appSettings);
        }

        /// <summary>
        /// 
        /// Ruft die Projektberechtigungen ab
        /// 
        /// </summary>
        public async Task<PermissionModel[]> GetPermissionsAsync(PermissionSource source)
        {
            List<PermissionModel> permissions = new List<PermissionModel>();

            switch (source)
            {
                case PermissionSource.ActiveDirectory:
                {
                    // ReadOnly
                    permissions.AddRange(GetUsersFromGroup(PermissionAccessRole.ReadOnly));

                    // ReadWrite
                    permissions.AddRange(GetUsersFromGroup(PermissionAccessRole.ReadWrite));

                    // Manager
                    permissions.AddRange(GetUsersFromGroup(PermissionAccessRole.Manager));

                    break;
                }
                case PermissionSource.File:
                {
                    // ReadOnly
                    permissions.AddRange(await GetUsersFromFileAsync(PermissionAccessRole.ReadOnly));

                    // ReadWrite
                    permissions.AddRange(await GetUsersFromFileAsync(PermissionAccessRole.ReadWrite));

                    // Manager
                    permissions.AddRange(await GetUsersFromFileAsync(PermissionAccessRole.Manager));

                    break;
                }
            }

            return permissions.ToArray();
        }

        private List<PermissionModel> GetUsersFromGroup(PermissionAccessRole accessRole)
        {
            string Name = GetDirectoryNameFromPath(ProjektPath);
            string groupName = AdUtil.GetAdGroupName(Name, GroupScope.Global, accessRole);
            PrincipalCollection groupMembers = AdUtil.GetGroupMembers(groupName, IdentityType.SamAccountName);

            if (null != groupMembers)
            {
                List<PermissionModel> permissions = new List<PermissionModel>();

                foreach (Principal userItem in groupMembers)
                {
                    if (userItem.GetType() == typeof(UserPrincipal))
                    {
                        permissions.Add(new PermissionModel()
                        {
                            User = new AdUser(userItem as UserPrincipal),
                            Source = PermissionSource.ActiveDirectory,
                            AccessRole = accessRole
                        });
                    }
                }

                return permissions;
            }

            return null;
        }

        private async Task<List<PermissionModel>> GetUsersFromFileAsync(PermissionAccessRole permissionAccess)
        {
            List<PermissionModel> projektPermissions = new List<PermissionModel>();

            // Get File Path
            string filePath = GetPermissionFilePath(permissionAccess, ProjektPath);

            // Get Content
            string[] fileContentLine = await GetFileContentAsync(filePath);

            if (null == fileContentLine)
                return projektPermissions; // Konnte die Berechtigungsdatei nicht lesen!

            // Filter Content: Unnessesary Lines
            string[] filteredFileContent = fileContentLine
               .Where(line => line.StartsWith("#") == false &&          // Kommentar entfernen
               string.IsNullOrWhiteSpace(line) == false &&              // Leere Zeile entfernen
               line.StartsWith("_") == false &&                         // Auskommentierungen entfernen
               line.ToUpper() != null)                                  // Umwandeln in Großbuchstaben
               ?.ToArray();                                             // In Array umwandeln

            // Filter Content: Doubled Usernames
            string[] tempContent = filteredFileContent.Select(line => line.ToUpper()).ToArray();
            if (tempContent.Distinct().Count() != tempContent.Count())
            {
                filteredFileContent = tempContent.Distinct()?.ToArray();
            }

            // Create Permission-Objects
            foreach (string fileLine in filteredFileContent)
            {
                AdUser user = new AdUser(fileLine);
                user.UpdateUser();

                if (user.IsValidated == true)
                {
                    projektPermissions.Add(new PermissionModel
                    {
                        AccessRole = permissionAccess,
                        User = user,
                        Source = PermissionSource.File
                    });
                }
            }

            return projektPermissions;
        }

        public string GetPermissionFilePath(PermissionAccessRole permissionAccess, string projektPath)
        {
            string fileName = string.Empty;
            switch (permissionAccess)
            {
                case PermissionAccessRole.ReadOnly:
                {
                    fileName = AppConstants.PermissionFileReadOnlyName;
                    break;
                }
                case PermissionAccessRole.ReadWrite:
                {
                    fileName = AppConstants.PermissionFileReadWriteName;
                    break;
                }
                case PermissionAccessRole.Manager:
                {
                    fileName = AppConstants.PermissionFileManagerName;
                    break;
                }
            }

            string organisationPath = string.Empty;
            switch (OrdnerVersion)
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


        /// <summary>
        /// 
        /// Fügt Berechtigungen dem Projekt hinzu
        /// 
        /// </summary>
        public async Task AddPermissionAsync(PermissionSource source, PermissionModel permission)
        {
            switch (source)
            {
                case PermissionSource.ActiveDirectory:
                {
                    AddUserToGroup(permission);
                    break;
                }
                case PermissionSource.File:
                {
                    await AddUserToFileAsync(permission);
                    break;
                }
            }
        }

        private void AddUserToGroup(PermissionModel permission)
        {
            if (null == permission)
                throw new ArgumentNullException();

            string Name = GetDirectoryNameFromPath(ProjektPath);
            string groupName = AdUtil.GetAdGroupName(Name, GroupScope.Global, permission.AccessRole);

            GroupPrincipal group = AdUtil.GetGroup(groupName, IdentityType.SamAccountName);
            UserPrincipal user = AdUtil.GetUser(permission.User.SamAccountName, IdentityType.SamAccountName);

            if (null != group)
            {
                if (null != user)
                {
                    group.Members.Add(user);
                    group.Save();
                }
                else
                {
                    throw new Exception($"Could not add permission for {permission.User.SamAccountName}. Username not found!");
                }
            }
            else
            {
                throw new Exception($"Could not add permission to group {groupName}. Group not found!");
            }
        }

        private async Task AddUserToFileAsync(PermissionModel permission)
        {
            if (null == permission)
                throw new ArgumentNullException();

            // Get FilePath
            string permissionFilePath = GetPermissionFilePath(permission.AccessRole, ProjektPath);

            // Get Content
            string[] fileContentLine = await GetFileContentAsync(permissionFilePath);

            // Add Entry
            Array.Resize(ref fileContentLine, fileContentLine.Length + 1);
            fileContentLine[fileContentLine.Length - 1] = permission.User.SamAccountName.ToUpper();

            // Write Content
            var fileContentAsString = string.Join(Environment.NewLine, fileContentLine);
            await WriteFileContentAsync(permissionFilePath, fileContentAsString);
        }


        /// <summary>
        /// 
        /// Entfernt Berechtigungen vom Projekt
        /// 
        /// </summary>
        public async Task RemovePermissionAsync(PermissionSource source, PermissionModel permission)
        {
            if (null == permission)
                return; // Kein Argument übergeben!

            switch (source)
            {
                case PermissionSource.ActiveDirectory:
                {
                    RemoveUserFromGroup(permission);
                    break;
                }
                case PermissionSource.File:
                {
                    await RemoveUserFromFileAsync(permission);
                    break;
                }
            }
        }

        private void RemoveUserFromGroup(PermissionModel permission)
        {
            if (null == permission)
                return; // Kein Argument übergeben!

            string Name = GetDirectoryNameFromPath(ProjektPath);
            string groupName = AdUtil.GetAdGroupName(Name, GroupScope.Global, permission.AccessRole);
            GroupPrincipal group = AdUtil.GetGroup(groupName, IdentityType.SamAccountName);
            Principal userItem = group.Members
                .Where(user => user.SamAccountName == permission.User.SamAccountName)
                .FirstOrDefault();

            if (null != group)
            {
                if (null != userItem)
                {
                    group.Members.Remove(userItem);
                    group.Save();
                }
                else
                {
                    throw new Exception($"Could not remove permission for {permission.User.SamAccountName}. Username not found!");
                }
            }
            else
            {
                throw new Exception($"Could not remove permission from group {groupName}. Group not found!");
            }
        }

        private async Task RemoveUserFromFileAsync(PermissionModel permission)
        {
            if (null == permission)
                return; // Kein Argument übergeben!

            // Get Content
            string filePath = GetPermissionFilePath(permission.AccessRole, ProjektPath);
            string[] fileContent = await GetFileContentAsync(filePath);

            // Remove Entry
            string removeableUsername = permission.User.SamAccountName.ToUpper();
            if (fileContent.Contains(removeableUsername))
            {
                fileContent[Array.IndexOf(fileContent, removeableUsername)] = "";   // Cleanup entry
                fileContent = fileContent                                           // Cleanup empty lines
                    .Where(line => string.IsNullOrWhiteSpace(line) == false)
                    .ToArray();
            }

            // Write Content
            var fileContentAsString = string.Join(Environment.NewLine, fileContent);
            await WriteFileContentAsync(filePath, fileContentAsString);
        }



        /// <summary>
        /// 
        /// Überschreibt die aktuellen Berechtigungen.
        /// 
        /// </summary>
        private async Task OverwritePermissionsAsync(PermissionSource source, PermissionModel[] permissions)
        {
            PermissionModel[] filePermissions = await GetPermissionsAsync(PermissionSource.File);

            // Remove all permissions
            foreach (PermissionModel permission in filePermissions)
            {
                await RemovePermissionAsync(PermissionSource.File, permission);
            }

            // Overwrite with new permissions
            foreach (PermissionModel permission in permissions)
            {
                await AddPermissionAsync(PermissionSource.File, permission);
            }
        }


        /// <summary>
        /// 
        /// Aktualisiert die Berechtigungen auf ein Projekt
        /// 
        /// </summary>
        public async Task UpdatePermissionsAsync(PermissionModel[] newPermissions)
        {
            List<Task> permissionTasks = new List<Task>();

            PermissionModel[] adPermissions = await GetPermissionsAsync(PermissionSource.ActiveDirectory);
            PermissionModel[] filePermissions = null;
            if (null != newPermissions)
            {
                filePermissions = newPermissions;

                // Overwrite file permissions
                permissionTasks.Add(
                    OverwritePermissionsAsync(PermissionSource.File, newPermissions));
            }
            else
            {
                filePermissions = await GetPermissionsAsync(PermissionSource.File);
            }


            // Add permissions
            IEnumerable<PermissionModel> permissionsToAdd = filePermissions.Except(adPermissions, new PermissionComparer());
            foreach (PermissionModel permission in permissionsToAdd)
            {
                permissionTasks.Add(
                    AddPermissionAsync(PermissionSource.ActiveDirectory, permission));
            }

            // Remove permissions
            IEnumerable<PermissionModel> permissionsToRemove = adPermissions.Except(filePermissions, new PermissionComparer());
            foreach (PermissionModel permission in permissionsToRemove)
            {
                permissionTasks.Add(
                    RemovePermissionAsync(PermissionSource.ActiveDirectory, permission));
            }

            await Task.WhenAll(permissionTasks.ToArray());
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public static string GetPermissionTemplate(PermissionAccessRole permissionAccess)
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







        // 
        // HELPERS
        //

        private string GetDirectoryNameFromPath(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            return directory.Name;
        }

        private async Task<string[]> GetFileContentAsync(string filePath)
        {
            string fileContent = string.Empty;
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
                fileContent = fileContent.ToUpper();
            }

            // Convert to lines
            string[] fileContentLine = fileContent.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None);

            return fileContentLine;
        }

        private async Task WriteFileContentAsync(string filePath, string content)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(content);
            }
        }

    }
}
