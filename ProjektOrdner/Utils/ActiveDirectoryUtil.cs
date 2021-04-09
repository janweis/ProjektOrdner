using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using static ProjektOrdner.Permission.AdUser;

namespace ProjektOrdner.Utils
{
    public class ActiveDirectoryUtil
    {
        private PrincipalContext GroupContext { get; set; }
        private PrincipalContext UserContext { get; set; }
        private AppSettings AppSettings { get; set; }

        public ActiveDirectoryUtil(AppSettings appSettings)
        {
            GroupContext = new PrincipalContext(ContextType.Domain, appSettings.AdDomainName, appSettings.AdGroupDlDN);
            UserContext = new PrincipalContext(ContextType.Domain, appSettings.AdDomainName);
            AppSettings = appSettings;
        }


        /// <summary>
        /// 
        /// </summary>
        public UserPrincipal GetUser(string value, IdentityType identity)
        {
            return UserPrincipal.FindByIdentity(UserContext, identity, value);
        }


        /// <summary>
        /// 
        /// </summary>
        public static DirectoryEntry GetUser(string value, IdentificationTypes type)
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
                    foundUser = UserPrincipal.FindByIdentity(UserContext, IdentityType.SamAccountName, value);
                    break;
                }
                case IdentificationTypes.Email:
                {
                    DirectoryEntry tempUser = GetUser(value, identification);
                    if (null == tempUser)
                        break;

                    foundUser = UserPrincipal.FindByIdentity(UserContext, IdentityType.DistinguishedName, tempUser.Properties["DistinguishedName"].Value.ToString());
                    break;
                }
                case IdentificationTypes.Matrikelnummer:
                {
                    DirectoryEntry tempUser = GetUser(value, identification);
                    if (null == tempUser)
                        break;

                    foundUser = UserPrincipal.FindByIdentity(UserContext, IdentityType.DistinguishedName, tempUser.Properties["DistinguishedName"].Value.ToString());
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
            GroupPrincipal group = GroupPrincipal.FindByIdentity(GroupContext, identity, identityValue);
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
        public string GetAdGroupName(string Name, GroupScope groupScope, PermissionRole accessRole)
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

            string NameWithoutSpaces = Name.Replace(" ", "");
            string adGroupName = $@"{AppSettings.AdGroupNamePrefix}{groupScopeName}{AppSettings.AdGroupNameTopic}{NameWithoutSpaces}{adGroupSuffix}";

            return adGroupName.ToUpper();
        }


        /// <summary>
        /// 
        /// </summary>
        public List<string> GetAdGroupNames(string Name)
        {
            return new List<string>
            {
                GetAdGroupName(Name, GroupScope.Local, PermissionRole.Guest),
                GetAdGroupName(Name, GroupScope.Local, PermissionRole.Member),
                GetAdGroupName(Name, GroupScope.Local, PermissionRole.Manager),
                GetAdGroupName(Name, GroupScope.Global, PermissionRole.Guest),
                GetAdGroupName(Name, GroupScope.Global, PermissionRole.Member),
                GetAdGroupName(Name, GroupScope.Global, PermissionRole.Manager)
            };
        }


        /// <summary>
        /// 
        /// </summary>
        public GroupPrincipal AddGroup(GroupScope scope, string samAccountName, string description)
        {
            GroupPrincipal adGroup = new GroupPrincipal(GroupContext, samAccountName)
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


        /// <summary>
        /// 
        /// </summary>
        public void AddGroupMembers(GroupPrincipal group, UserPrincipal user)
        {
            if (null != group)
            {
                if (null != user)
                {
                    if (group.Members.Contains(user) == false)
                        group.Members.Add(user);
                }
            }
        }


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
                        group.Members.Remove(user);
                }
            }
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

    }
}
