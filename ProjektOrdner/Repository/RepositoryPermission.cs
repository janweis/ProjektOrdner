using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace ProjektOrdner.Repository
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

        public RepositoryPermission(AdUser user, PermissionRole role, PermissionSource source, AppSettings settings)
        {
            User = user;
            Role = role;
            Source = source;
            Settings = settings;

            DirectoryUtil = new ActiveDirectoryUtil(settings);
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 

        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 

        // // // // // // // // // // // // // // // // // // // // //
        // Active Directory IO
        //

        /// <summary>
        /// 
        /// </summary>
        private PermissionRole GetPermissionRoleFromActiveDirectory(RepositoryOrganization organization)
        {
            UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

            // ReadWrite
            string adReadWriteGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Member);
            GroupPrincipal adReadWriteGroup = DirectoryUtil.GetGroup(adReadWriteGroupName, IdentityType.SamAccountName);

            if (adReadWriteGroup.Members.Contains(adUser) == true)
                return PermissionRole.Member;

            // Manager
            string adManagerGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Manager);
            GroupPrincipal adManagerGroup = DirectoryUtil.GetGroup(adManagerGroupName, IdentityType.SamAccountName);

            if (adManagerGroup.Members.Contains(adUser) == true)
                return PermissionRole.Manager;

            // ReadOnly
            string adReadOnlyGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, PermissionRole.Guest);
            GroupPrincipal adReadOnlyGroup = DirectoryUtil.GetGroup(adManagerGroupName, IdentityType.SamAccountName);

            if (adReadOnlyGroup.Members.Contains(adUser) == true)
                return PermissionRole.Guest;

            return PermissionRole.Undefined;
        }


        /// <summary>
        /// 
        /// </summary>
        private void AddUserToADGroup(RepositoryOrganization organization)
        {
            string adGroupName = DirectoryUtil.GetAdGroupName(organization.ProjektName, GroupScope.Global, Role);
            if (string.IsNullOrWhiteSpace(adGroupName) == true)
            {
                throw new Exception($"Could not get Active Directory Group Name '{organization.ProjektName}'!");
            }
            else
            {
                GroupPrincipal adGroup = DirectoryUtil.GetGroup(adGroupName, IdentityType.SamAccountName);

                if (null == adGroup)
                {
                    throw new Exception($"Could not get Active Directory Group Object '{adGroupName}'!");
                }
                else
                {
                    UserPrincipal adUser = DirectoryUtil.GetUser(User.SamAccountName, IdentityType.SamAccountName);

                    if (null == adUser)
                    {
                        throw new Exception($"Could not get Active Directory User Object '{User.SamAccountName}'!");
                    }
                    else
                    {
                        if (adGroup.Members.Contains(adUser) == false)
                        {
                            adGroup.Members.Add(adUser);
                            adGroup.Save();
                        }
                    }
                }
            }
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


        // // // // // // // // // // // // // // // // // // // // //
        // File IO
        //

        /// <summary>
        /// 
        /// </summary>
        private async Task<PermissionRole> GetPermissionRoleFromFilesAsync(string projektPath, RepositoryVersion version)
        {
            // ReadWrite
            string readWriteFileContent = await ReadFileContent(Path.Combine(projektPath, GetPermissionFileName(PermissionRole.Member, version)));

            if (readWriteFileContent.Contains(User.SamAccountName) == true)
                return PermissionRole.Member;

            // Manager
            string managerFileContent = await ReadFileContent(Path.Combine(projektPath, GetPermissionFileName(PermissionRole.Manager, version)));

            if (managerFileContent.Contains(User.SamAccountName) == true)
                return PermissionRole.Manager;

            // ReadOnly
            string readOnlyFileContent = await ReadFileContent(Path.Combine(projektPath, GetPermissionFileName(PermissionRole.Guest, version)));

            if (readOnlyFileContent.Contains(User.SamAccountName) == true)
                return PermissionRole.Guest;

            return PermissionRole.Undefined;
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task AddUserToPermissionFileV2(string projektPath)
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
        private async Task AddUserToPermissionFileV1(string projektPath)
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
        /// Ließt Daten aus einer Datei.
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
        /// Schreibt Daten in eine Datei.
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
