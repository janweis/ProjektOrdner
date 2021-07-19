using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.Threading.Tasks;
using static ProjektOrdner.Permission.AdUser;

namespace ProjektOrdner.Utils
{
    public class ActiveDirectoryUtil
    {
        private PrincipalContext PrincipalContext { get; set; }
        private AppSettings AppSettings { get; set; }

        public ActiveDirectoryUtil(AppSettings appSettings)
        {
            PrincipalContext = new PrincipalContext(ContextType.Domain, appSettings.AdDomainName);
            AppSettings = appSettings;
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // GET
        //


        /// <summary>
        /// 
        /// </summary>
        public UserPrincipal GetUser(string value, IdentityType identity)
        {
            return UserPrincipal.FindByIdentity(PrincipalContext, identity, value);
        }


        /// <summary>
        /// 
        /// </summary>
        public static DirectoryEntry Get(string value, IdentificationTypes type)
        {
            string ldapFilter = GetLdapFilter(value, type);
            string ldapPath = GetLdapPath();

            using (DirectoryEntry entry = new DirectoryEntry(ldapPath))
            {
                using (DirectorySearcher directorySearcher = new DirectorySearcher(entry, ldapFilter, new string[] { "employeeNumber" }))
                {
                    SearchResult searchResult = directorySearcher.FindOne();

                    if (null != searchResult)
                    {
                        return searchResult.GetDirectoryEntry();
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public UserPrincipal GetUserByType(string value, IdentificationTypes identification)
        {
            UserPrincipal foundUser = null;
            switch (identification)
            {
                case IdentificationTypes.SamAccountName:
                {
                    foundUser = UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.SamAccountName, value);
                    break;
                }
                case IdentificationTypes.Email:
                {
                    DirectoryEntry tempUser = Get(value, identification);
                    if (null == tempUser)
                        break;

                    foundUser = UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.DistinguishedName, tempUser.Properties["DistinguishedName"].Value.ToString());
                    break;
                }
                case IdentificationTypes.Matrikelnummer:
                {
                    DirectoryEntry tempUser = Get(value, identification);
                    if (null == tempUser)
                        break;

                    foundUser = UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.DistinguishedName, tempUser.Properties["DistinguishedName"].Value.ToString());
                    break;
                }
            }

            return foundUser;
        }


        /// <summary>
        /// 
        /// </summary>
        public GroupPrincipal GetGroup(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = GroupPrincipal.FindByIdentity(PrincipalContext, identity, identityValue);
            return group;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<GroupPrincipal> GetGroupAsync(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = null;

            await Task.Run((Action)(() =>
            {
                group = GroupPrincipal.FindByIdentity((PrincipalContext)this.PrincipalContext, identity, identityValue);

            }));

            return group;
        }



        /// <summary>
        /// 
        /// </summary>
        public DirectoryEntry GetGroup(string value)
        {
            string ldapFilter = $"(&(objectclass=group)(name={value}))";
            string ldapPath = GetLdapPath();

            using (DirectoryEntry entry = new DirectoryEntry(ldapPath))
            {
                using (DirectorySearcher directorySearcher = new DirectorySearcher(entry, ldapFilter, new string[] { "employeeNumber" }))
                {
                    SearchResult searchResult = directorySearcher.FindOne();

                    if (null != searchResult)
                    {
                        return searchResult.GetDirectoryEntry();
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<GroupPrincipal> GetAdGroups(string Name)
        {
            List<GroupPrincipal> groupList = new List<GroupPrincipal>();

            foreach (string groupNameItem in GetAdGroupNames(Name))
            {
                GroupPrincipal group = GetGroup(groupNameItem, IdentityType.SamAccountName);

                if (group != null)
                {
                    groupList.Add(group);
                }
            }

            return groupList;
        }


        /// <summary>
        /// 
        /// </summary>
        public PrincipalCollection GetGroupMembers(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = GetGroup(identityValue, identity);

            if (group != null)
            {
                return group.Members;
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public string GetAdGroupName(string repositoryName, GroupScope groupScope, PermissionRole accessRole)
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
            switch (accessRole)
            {
                case PermissionRole.Manager:
                {
                    adGroupSuffix = AppSettings.AdGroupNameSuffixManager;
                    break;
                }
                case PermissionRole.Guest:
                {
                    adGroupSuffix = AppSettings.AdGroupNameSuffixRead;
                    break;
                }
                case PermissionRole.Member:
                {
                    adGroupSuffix = AppSettings.AdGroupNameSuffixWrite;
                    break;
                }
            }

            string NameWithoutSpaces = repositoryName.Replace(" ", "");
            string adGroupName = $@"{AppSettings.AdGroupNamePrefix}{groupScopeName}{AppSettings.AdGroupNameTopic}{NameWithoutSpaces}{adGroupSuffix}";

            return adGroupName.ToUpper();
        }


        /// <summary>
        /// 
        /// </summary>
        public List<string> GetAdGroupNames(string repositoryName)
        {
            return new List<string>
            {
                GetAdGroupName(repositoryName, GroupScope.Local, PermissionRole.Guest),
                GetAdGroupName(repositoryName, GroupScope.Local, PermissionRole.Member),
                GetAdGroupName(repositoryName, GroupScope.Local, PermissionRole.Manager),
                GetAdGroupName(repositoryName, GroupScope.Global, PermissionRole.Guest),
                GetAdGroupName(repositoryName, GroupScope.Global, PermissionRole.Member),
                GetAdGroupName(repositoryName, GroupScope.Global, PermissionRole.Manager)
            };
        }


        /// <summary>
        /// 
        /// Ruft die SID der Domäne ab.
        /// 
        /// </summary>
        public static SecurityIdentifier GetDomainSID()
        {
            byte[] domainSid;

            var directoryContext =
                new DirectoryContext(DirectoryContextType.Domain, System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName);

            using (Domain domain = Domain.GetDomain(directoryContext))
            using (DirectoryEntry directoryEntry = domain.GetDirectoryEntry())
                domainSid = (byte[])directoryEntry.Properties["objectSid"].Value;

            SecurityIdentifier sid = new SecurityIdentifier(domainSid, 0);
            //bool validUser = UserPrincipal.Current.Sid.IsEqualDomainSid(sid);

            return sid;
        }


        /// <summary>
        /// 
        /// </summary>
        public static string GetLdapPath()
        {
            string ldapString = "";
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string[] splittedDomainName = domainName.Split('.');

            // Add ldap dns syntax
            for (int i = 0; i <= (splittedDomainName.Length - 1); i++)
            {
                string ldapTemp = "";

                if (i < (splittedDomainName.Length - 1))
                {
                    ldapTemp = "DC=" + splittedDomainName[i] + ",";
                }
                else
                {
                    ldapTemp = "DC=" + splittedDomainName[i];
                }

                ldapString = ldapString + ldapTemp;
            }

            // Add protocol
            ldapString = ldapString.Insert(0, "LDAP://");

            return ldapString;
        }


        /// <summary>
        /// 
        /// </summary>
        private static string GetLdapFilter(string value, IdentificationTypes type)
        {
            switch (type)
            {
                case IdentificationTypes.SamAccountName:
                {
                    return $"(&(objectCategory=person)(objectClass=user)(samAccountName={value}))";
                }

                case IdentificationTypes.Email:
                {
                    return $"(&(objectCategory=person)(objectClass=user)(mail={value}))";
                }

                case IdentificationTypes.Matrikelnummer:
                {
                    return $"(&(objectCategory=person)(objectClass=user)(employeeNumber={value}))";
                }

                default:
                {
                    return "";
                }
            }
        }



        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // NEW
        //


        /// <summary>
        /// 
        /// </summary>
        public GroupPrincipal NewAdGroup(GroupScope scope, string samAccountName, string description)
        {
            GroupPrincipal adGroup = new GroupPrincipal(PrincipalContext, samAccountName)
            {
                // Type
                GroupScope = scope,
                IsSecurityGroup = true,

                // Definitions
                Name = samAccountName,
                DisplayName = samAccountName,
                Description = description,
            };

            // Invoke
            adGroup.Save();

            return adGroup;
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // ADD
        //


        /// <summary>
        /// 
        /// </summary>
        public void AddGroupMembers(GroupPrincipal group, UserPrincipal userToAdd)
        {
            if (null != group)
            {
                if (null != userToAdd)
                {
                    if (group.Members.Contains(userToAdd) == false)
                    {
                        group.Members.Add(userToAdd);
                        group.Save();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void AddGroupMembers(GroupPrincipal group, GroupPrincipal groupToAdd)
        {
            if (null != group)
            {
                if (null != groupToAdd)
                {
                    if (group.Members.Contains(groupToAdd) == false)
                    {
                        group.Members.Add(groupToAdd);
                        group.Save();
                    }
                }
            }
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // REMOVE
        //


        /// <summary>
        /// 
        /// </summary>
        public void DeleteUser(string identityValue, IdentityType identity)
        {
            UserPrincipal user = GetUser(identityValue, identity);

            if (user != null)
            {
                user.Delete();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void DeleteGroup(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = GetGroup(identityValue, identity);

            if (group != null)
            {
                group.Delete();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveGroupMember(GroupPrincipal group, UserPrincipal user)
        {
            if (null != group)
            {
                if (null != user)
                {
                    if (group.Members.Contains(user) == true)
                    {
                        group.Members.Remove(user);
                        group.Save();
                    }
                }
            }
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // 
        // RENAME
        //


        /// <summary>
        /// 
        /// Benennt die Active Directory Gruppe um
        /// 
        /// </summary>
        public async Task RenameGroupAsync(string groupName, string newName)
        {
            GroupPrincipal adGroup = await GetGroupAsync(groupName, IdentityType.SamAccountName);

            if (null != adGroup)
                await RenameGroupAsync(adGroup, newName);
        }


        /// <summary>
        /// 
        /// Benennt die Active Directory Gruppe um
        /// 
        /// </summary>
        public async static Task RenameGroupAsync(GroupPrincipal group, string newName)
        {
            await Task.Run(() =>
            {
                if (null != group)
                {
                    var Groupentry = (DirectoryEntry)group.GetUnderlyingObject();
                    Groupentry.Rename($"cn={newName}");
                    Groupentry.CommitChanges();
                }
            });
        }


    }
}
