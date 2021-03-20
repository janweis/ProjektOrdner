using ProjektOrdner.App;
using ProjektOrdner.Repository;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjektOrdner.Permission
{
    public class RepositoryPermission
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        public AdUser User { get; set; }
        public PermissionRole Role { get; set; }
        public PermissionSource Source { get; set; }


        private AppSettings Settings { get; set; }
        private ActiveDirectoryUtil DirectoryUtil { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public RepositoryPermission(AppSettings settings)
        {
            Settings = settings;
            DirectoryUtil = new ActiveDirectoryUtil(settings);
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 

        /// <summary>
        /// 
        /// </summary>
        public async Task AddToRepositoryAsync(RepositoryFolder repository)
        {
            switch (repository.Version)
            {
                case RepositoryVersion.V1:
                {
                    await AddToFileV1Async(repository.Organization.ProjektPath);
                    AddToActiveDirectoryV1(repository.Organization);
                    break;
                }
                case RepositoryVersion.V2:
                {
                    await AddToFileV2Async(repository.Organization.ProjektPath);
                    AddToActiveDirectoryV2(repository.Organization);
                    break;
                }
                case RepositoryVersion.Unknown:
                {
                    throw new ArgumentException("Unknown Repository Version!");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task AddToRepositoryAsync(string projektPath)
        {
            RepositoryFolder repository = new RepositoryFolder(Settings);
            await repository.Load(projektPath);
            await AddToRepositoryAsync(repository);
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task RemoveFromRepositoryAsync(RepositoryFolder repository)
        {
            switch (repository.Version)
            {
                case RepositoryVersion.V1:
                {
                    await RemoveFromFileV1Async(repository.Organization.ProjektPath);
                    RemoveFromActiveDirectoryV1(repository.Organization);
                    break;
                }
                case RepositoryVersion.V2:
                {
                    await RemoveFromFileV2Async(repository.Organization.ProjektPath);
                    RemoveFromActiveDirectoryV2(repository.Organization);
                    break;
                }
                case RepositoryVersion.Unknown:
                {
                    throw new ArgumentException("Unknown Repository Version!");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task RemoveFromRepositoryAsync(string projektPath)
        {
            RepositoryFolder repository = new RepositoryFolder(Settings);
            await repository.Load(projektPath);
            await RemoveFromRepositoryAsync(repository);
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task UpdatePermission(RepositoryFolder repository)
        {
            PermissionRole fileRole = await GetPermissionRoleFromFilesAsync(repository.Organization.ProjektPath, repository.Organization.Version);

            //PermissionRole adRole = GetPermissionRoleFromActiveDirectory(repository.Organization);

            // Remove User
            //if(fileRole == PermissionRole.Undefined && adRole != PermissionRole.Undefined)
            //{
            //    if(repository.Organization.Version == RepositoryVersion.V1)
            //    {
            //        RemoveFromActiveDirectoryV1(repository.Organization);
            //    }
            //    else if(repository.Organization.Version == RepositoryVersion.V2)
            //    {
            //        RemoveFromActiveDirectoryV2(repository.Organization);
            //    }
            //    else
            //    {
            //        throw new ArgumentException("Unknown Repository Version!");
            //    }

            //    return;
            //}

            // Add User
            //if(fileRole != PermissionRole.Undefined && adRole == PermissionRole.Undefined)
            //{
            //    if (repository.Organization.Version == RepositoryVersion.V1)
            //    {
            //        AddToActiveDirectoryV1(repository.Organization);
            //    }
            //    else if (repository.Organization.Version == RepositoryVersion.V2)
            //    {
            //        AddToActiveDirectoryV2(repository.Organization);
            //    }
            //    else
            //    {
            //        throw new ArgumentException("Unknown Repository Version!");
            //    }

            //    return;
            //}

            // If User has new Permission
            if (Role != fileRole)
            {
                switch (repository.Version)
                {
                    case RepositoryVersion.V1:
                    {
                        await RemoveFromFileV1Async(repository.Organization.ProjektPath, fileRole);
                        RemoveFromActiveDirectoryV1(repository.Organization, fileRole);

                        await AddToFileV1Async(repository.Organization.ProjektPath);
                        AddToActiveDirectoryV1(repository.Organization);

                        break;
                    }
                    case RepositoryVersion.V2:
                    {
                        await RemoveFromFileV2Async(repository.Organization.ProjektPath, fileRole);
                        RemoveFromActiveDirectoryV2(repository.Organization, fileRole);

                        await AddToFileV2Async(repository.Organization.ProjektPath);
                        AddToActiveDirectoryV2(repository.Organization);

                        break;
                    }
                    case RepositoryVersion.Unknown:
                        throw new ArgumentException("Unknown Repository Version!");
                }
            }
        }



        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 


        /// <summary>
        /// 
        /// </summary>
        private PermissionRole GetPermissionRoleFromActiveDirectory(RepositoryOrganization organization)
        {
            UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

            // ReadWrite
            string adReadWriteGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.ReadWrite);
            GroupPrincipal adReadWriteGroup = DirectoryUtil.GetGroup(adReadWriteGroupName, IdentityType.SamAccountName);

            if (adReadWriteGroup.Members.Contains(adUser) == true)
                return PermissionRole.ReadWrite;

            // Manager
            string adManagerGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Manager);
            GroupPrincipal adManagerGroup = DirectoryUtil.GetGroup(adManagerGroupName, IdentityType.SamAccountName);

            if (adManagerGroup.Members.Contains(adUser) == true)
                return PermissionRole.Manager;

            // ReadOnly
            string adReadOnlyGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.ReadOnly);
            GroupPrincipal adReadOnlyGroup = DirectoryUtil.GetGroup(adManagerGroupName, IdentityType.SamAccountName);

            if (adReadOnlyGroup.Members.Contains(adUser) == true)
                return PermissionRole.ReadOnly;

            return PermissionRole.Undefined;
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task<PermissionRole> GetPermissionRoleFromFilesAsync(string projektPath, RepositoryVersion version)
        {
            // ReadWrite
            string readWriteFileContent = await ReadFileContent(Path.Combine(projektPath, GetPermissionFileName(PermissionRole.ReadWrite, version)));

            if (readWriteFileContent.Contains(User.SamAccountName) == true)
                return PermissionRole.ReadWrite;

            // Manager
            string managerFileContent = await ReadFileContent(Path.Combine(projektPath, GetPermissionFileName(PermissionRole.Manager, version)));

            if (managerFileContent.Contains(User.SamAccountName) == true)
                return PermissionRole.Manager;

            // ReadOnly
            string readOnlyFileContent = await ReadFileContent(Path.Combine(projektPath, GetPermissionFileName(PermissionRole.ReadOnly, version)));

            if (readOnlyFileContent.Contains(User.SamAccountName) == true)
                return PermissionRole.ReadOnly;

            return PermissionRole.Undefined;
        }


        /// <summary>
        /// 
        /// </summary>
        private void AddToActiveDirectoryV2(RepositoryOrganization organization)
        {
            string adGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, Role);
            GroupPrincipal adGroup = DirectoryUtil.GetGroup(adGroupName, IdentityType.SamAccountName);
            UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

            if (adGroup.Members.Contains(adUser) == false)
            {
                adGroup.Members.Add(adUser);
                adGroup.Save();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task AddToFileV2Async(string projektPath)
        {
            string filePath = Path.Combine(projektPath, Path.Combine(AppConstants.OrganisationFolderName, GetPermissionFileName(Role, RepositoryVersion.V2)));
            string fileContent = await ReadFileContent(filePath);

            if (fileContent.Contains(User.SamAccountName) == false)
            {
                fileContent = fileContent + Environment.NewLine + User.SamAccountName;
            }

            await WriteFileContent(filePath, fileContent);
        }


        /// <summary>
        /// 
        /// </summary>
        private void AddToActiveDirectoryV1(RepositoryOrganization organization)
        {
            string adGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, Role);
            GroupPrincipal adGroup = DirectoryUtil.GetGroup(adGroupName, IdentityType.SamAccountName);
            UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

            if (adGroup.Members.Contains(adUser) == false)
            {
                adGroup.Members.Add(adUser);
                adGroup.Save();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task AddToFileV1Async(string projektPath)
        {
            string filePath = Path.Combine(projektPath, GetPermissionFileName(Role, RepositoryVersion.V1));
            string fileContent = await ReadFileContent(filePath);

            if (fileContent.Contains(User.SamAccountName) == false)
            {
                fileContent = fileContent + Environment.NewLine + User.SamAccountName;
            }

            await WriteFileContent(filePath, fileContent);
        }


        /// <summary>
        /// 
        /// </summary>
        private void RemoveFromActiveDirectoryV2(RepositoryOrganization organization) =>
            RemoveFromActiveDirectoryV2(organization, Role);


        /// <summary>
        /// 
        /// </summary>
        private void RemoveFromActiveDirectoryV2(RepositoryOrganization organization, PermissionRole role)
        {
            string adGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, role);
            GroupPrincipal adGroup = DirectoryUtil.GetGroup(adGroupName, IdentityType.SamAccountName);
            UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

            if (adGroup.Members.Contains(adUser) == true)
            {
                adGroup.Members.Remove(adUser);
                adGroup.Save();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task RemoveFromFileV2Async(string projektPath) =>
            await RemoveFromFileV2Async(projektPath, Role);


        /// <summary>
        /// 
        /// </summary>
        private async Task RemoveFromFileV2Async(string projektPath, PermissionRole role)
        {
            string filePath = Path.Combine(projektPath, Path.Combine(AppConstants.OrganisationFolderName, GetPermissionFileName(role, RepositoryVersion.V2)));
            string fileContent = await ReadFileContent(filePath);

            if (fileContent.Contains(User.SamAccountName) == true)
            {
                fileContent = fileContent.Replace(User.SamAccountName, "");
            }

            await WriteFileContent(filePath, fileContent);
        }


        /// <summary>
        /// 
        /// </summary>
        private void RemoveFromActiveDirectoryV1(RepositoryOrganization organization) =>
            RemoveFromActiveDirectoryV1(organization, Role);


        /// <summary>
        /// 
        /// </summary>
        private void RemoveFromActiveDirectoryV1(RepositoryOrganization organization, PermissionRole role)
        {
            string adGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, role);
            GroupPrincipal adGroup = DirectoryUtil.GetGroup(adGroupName, IdentityType.SamAccountName);
            UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

            if (adGroup.Members.Contains(adUser) == true)
            {
                adGroup.Members.Remove(adUser);
                adGroup.Save();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task RemoveFromFileV1Async(string projektPath) =>
            await RemoveFromFileV1Async(projektPath, Role);


        /// <summary>
        /// 
        /// </summary>
        private async Task RemoveFromFileV1Async(string projektPath, PermissionRole role)
        {
            string filePath = Path.Combine(projektPath, Path.Combine(AppConstants.OrganisationFolderName, GetPermissionFileName(role, RepositoryVersion.V1)));
            string fileContent = await ReadFileContent(filePath);

            if (fileContent.Contains(User.SamAccountName) == true)
            {
                fileContent = fileContent.Replace(User.SamAccountName, "");
            }

            await WriteFileContent(filePath, fileContent);
        }


        /// <summary>
        /// 
        /// </summary>
        private string GetPermissionFileName(PermissionRole role, RepositoryVersion version)
        {
            string fileName = "";
            switch (version)
            {
                case RepositoryVersion.V1:
                {
                    switch (role)
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


        // // // // // // // // // // // // // // // // // // // // //
        // File IO
        // 

        /// <summary>
        /// 
        /// </summary>
        private async Task<string> ReadFileContent(string filePath)
        {
            string fileContent;
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            return fileContent;
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task WriteFileContent(string filePath, string fileContent)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(fileContent);
            }
        }


    }
}
