using ProjektOrdner.App;
using ProjektOrdner.Repository;
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
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        private AppSettings AppSettings { get; set; }
        private ActiveDirectoryUtil AdUtil { get; set; }
        private string ProjektPath { get; set; }
        private RepositoryVersion OrdnerVersion { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

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


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 

        /// <summary>
        /// 
        /// Ruft die Projektberechtigungen ab
        /// 
        /// </summary>
        public async Task<RepositoryPermission[]> GetPermissionsAsync(PermissionSource source)
        {
            List<RepositoryPermission> permissions = new List<RepositoryPermission>();

            switch (source)
            {
                case PermissionSource.ActiveDirectory:
                {
                    permissions.AddRange(GetUsersFromGroup(PermissionRole.ReadOnly));
                    permissions.AddRange(GetUsersFromGroup(PermissionRole.ReadWrite));
                    permissions.AddRange(GetUsersFromGroup(PermissionRole.Manager));

                    break;
                }
                case PermissionSource.File:
                {
                    permissions.AddRange(await GetUsersFromFileAsync(PermissionRole.ReadOnly));
                    permissions.AddRange(await GetUsersFromFileAsync(PermissionRole.ReadWrite));
                    permissions.AddRange(await GetUsersFromFileAsync(PermissionRole.Manager));

                    break;
                }
            }

            return permissions.ToArray();
        }



        /// <summary>
        /// 
        /// </summary>
        public string GetPermissionFilePath(PermissionRole permissionAccess, string projektPath)
        {
            string fileName = string.Empty;
            switch (permissionAccess)
            {
                case PermissionRole.ReadOnly:
                {
                    fileName = AppConstants.PermissionFileReadOnlyName;
                    break;
                }
                case PermissionRole.ReadWrite:
                {
                    fileName = AppConstants.PermissionFileReadWriteName;
                    break;
                }
                case PermissionRole.Manager:
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
        /// Aktualisiert die Berechtigungen auf ein Projekt
        /// 
        /// </summary>
        public async Task UpdatePermissionsAsync(RepositoryPermission[] newPermissions)
        {
            List<Task> permissionTasks = new List<Task>();

            RepositoryPermission[] adPermissions = await GetPermissionsAsync(PermissionSource.ActiveDirectory);
            RepositoryPermission[] filePermissions = null;
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
            IEnumerable<RepositoryPermission> permissionsToAdd = filePermissions.Except(adPermissions, new PermissionComparer());
            foreach (RepositoryPermission permission in permissionsToAdd)
            {
                permission.AddToRepositoryAsync

                permissionTasks.Add(
                    AddPermissionAsync(PermissionSource.ActiveDirectory, permission));
            }

            // Remove permissions
            IEnumerable<RepositoryPermission> permissionsToRemove = adPermissions.Except(filePermissions, new PermissionComparer());
            foreach (RepositoryPermission permission in permissionsToRemove)
            {
                permissionTasks.Add(
                    RemovePermissionAsync(PermissionSource.ActiveDirectory, permission));
            }

            await Task.WhenAll(permissionTasks.ToArray());
        }


        /// <summary>
        /// 
        /// </summary>
        public static string GetPermissionTemplate(PermissionRole permissionAccess)
        {
            string accessType = "";
            switch (permissionAccess)
            {
                case PermissionRole.ReadOnly:
                {
                    accessType = "Nur Lesen";
                    break;
                }
                case PermissionRole.ReadWrite:
                {
                    accessType = "Lesen & Schreiben";
                    break;
                }
                case PermissionRole.Manager:
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







        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 


    }
}
