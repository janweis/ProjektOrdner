using ProjektOrdner.Utils;
using System;
using System.DirectoryServices;

namespace ProjektOrdner.Permission
{
    public class UserProcessor
    {
        private UserModel User { get; set; }
        private readonly string LdapString = ActiveDirectoryUtil.GetLdapPath();

        public UserProcessor(UserModel user)
        {
            if (null == user)
                throw new ArgumentNullException();

            User = user;
        }

        public void Validate()
        {
            if (User.IsValidated)
                return;

            if (string.IsNullOrWhiteSpace(User.Identification))
                return;

            DirectoryEntry foundUser = ActiveDirectoryUtil.GetUser(User.Identification, User.IdentificationType);

            if (null != foundUser)
            {
                User.IsValidated = true;
            }
        }

        public void UpdateUserData()
        {
            DirectoryEntry foundUser = ActiveDirectoryUtil.GetUser(User.Identification, User.IdentificationType);

            if (null == foundUser)
                return;

            User.Vorname = foundUser.Properties["givenName"].Value?.ToString();
            User.Nachname = foundUser.Properties["sn"].Value?.ToString();
            User.SamAccountName = foundUser.Properties["sAMAccountName"].Value?.ToString();
            User.EMail = foundUser.Properties["mail"].Value?.ToString();

            if (foundUser.Properties["employeeNumber"].Value != null)
            {
                if (uint.TryParse(foundUser.Properties["employeeNumber"].Value.ToString(), out uint result))
                {
                    User.Matrikelnummer = result;
                }
            }

            // Validation not needed, cause User exists
            User.IsValidated = true;
        }

    }
}
