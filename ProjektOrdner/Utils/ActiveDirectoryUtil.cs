using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using static ProjektOrdner.Permission.UserModel;

namespace ProjektOrdner.Utils
{
    public class ActiveDirectoryUtil
    {
        private PrincipalContext GroupContext { get; set; }
        private PrincipalContext UserContext { get; set; }
        private AppSettings AppSettings { get; set; }

        public ActiveDirectoryUtil(AppSettings appSettings)
        {
            GroupContext = new PrincipalContext(ContextType.Domain, Environment.UserDomainName, appSettings.AdGroupDlDN);
            UserContext = new PrincipalContext(ContextType.Domain, Environment.UserDomainName);
            AppSettings = appSettings;
        }


        //
        // GET
        // -----------------------------------------------------------------------------------------------

        public UserPrincipal GetUser(string value, IdentityType identity)
        {
            return UserPrincipal.FindByIdentity(UserContext, identity, value);
        }

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

        public UserPrincipal GetUserByType(string value, IdentificationTypes identification)
        {
            UserPrincipal foundUser = null;
            switch (identification)
            {
                case IdentificationTypes.SamAccountName:
                {
                    foundUser = UserPrincipal.FindByIdentity(UserContext,IdentityType.SamAccountName, value);
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


        public GroupPrincipal GetGroup(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = GroupPrincipal.FindByIdentity(GroupContext, identity, identityValue);
            return group;
        }

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

        public List<GroupPrincipal> GetAdGroups(string projektName)
        {
            List<GroupPrincipal> groupList = new List<GroupPrincipal>();

            foreach (string groupNameItem in GetAdGroupNames(projektName))
            {
                GroupPrincipal group = GetGroup(groupNameItem, IdentityType.SamAccountName);

                if (group != null)
                {
                    groupList.Add(group);
                }
            }

            return groupList;
        }

        public PrincipalCollection GetGroupMembers(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = GetGroup(identityValue, identity);

            if (group != null)
            {
                return group.Members;
            }

            return null;
        }

        public string GetAdGroupName(string projektName, GroupScope groupScope, PermissionAccessRole accessRole)
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

        public List<string> GetAdGroupNames(string projektName)
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



        //
        // ADD
        // -----------------------------------------------------------------------------------------------

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

        public void AddGroupMembers(GroupPrincipal group, UserPrincipal user)
        {
            if (null != group)
            {
                if (null != user)
                {
                    group.Members.Add(user);
                }
            }
        }



        //
        // REMOVE
        // -----------------------------------------------------------------------------------------------

        public void DeleteUser(string identityValue, IdentityType identity)
        {
            UserPrincipal user = GetUser(identityValue, identity);

            if (user != null)
            {
                user.Delete();
            }
        }

        public void RemoveGroup(string identityValue, IdentityType identity)
        {
            GroupPrincipal group = GetGroup(identityValue, identity);

            if (group != null)
            {
                group.Delete();
            }
        }

        public void RemoveGroupMember(GroupPrincipal group, UserPrincipal user)
        {
            if (group != null)
            {
                group.Members.Remove(user);
            }
        }



        //
        // HELPERS
        // ___________________________________________________________________________________________

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
