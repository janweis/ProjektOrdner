using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public class AdUser
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variablen
        // 

        public enum IdentificationTypes { SamAccountName, Email, Matrikelnummer }

        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string SamAccountName { get; set; }
        public string Email { get; set; }
        public uint Matrikelnummer { get; set; }
        public string UserPrincipalName { get; set; }

        public IdentificationTypes IdentificationType { get; }
        public string Identification { get; set; }
        public bool IsValidated { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public AdUser(UserPrincipal user)
        {
            if (null == user)
                throw new ArgumentNullException("user");

            Vorname = user.GivenName;
            Nachname = user.Surname;
            Email = user.EmailAddress;
            SamAccountName = user.SamAccountName;
            UserPrincipalName = user.UserPrincipalName;

            using (DirectoryEntry directoryEntry = (DirectoryEntry)user.GetUnderlyingObject())
            {
                if (directoryEntry.Properties["employeeNumber"].Value != null)
                {
                    if (null != directoryEntry.Properties["employeeNumber"].Value)
                    {
                        Matrikelnummer = uint.Parse(directoryEntry.Properties["employeeNumber"].Value.ToString());
                    }
                }
            }
        }

        public AdUser(string identString)
        {
            Identification = identString;
            IdentificationType = GetIdentType(Identification);

            switch (IdentificationType)
            {
                case IdentificationTypes.SamAccountName:
                    SamAccountName = identString;
                    break;

                case IdentificationTypes.Email:
                    Email = identString;
                    break;

                case IdentificationTypes.Matrikelnummer:
                    Matrikelnummer = uint.Parse(identString);
                    break;
            }

            IsValidated = false;
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 


        /// <summary>
        /// 
        /// </summary>
        public void UpdateUserData()
        {
            DirectoryEntry foundUser = ActiveDirectoryUtil.GetUser(Identification, IdentificationType);

            if (null == foundUser)
                return;

            Vorname = foundUser.Properties["givenName"].Value?.ToString();
            Nachname = foundUser.Properties["sn"].Value?.ToString();
            SamAccountName = foundUser.Properties["sAMAccountName"].Value?.ToString();
            Email = foundUser.Properties["mail"].Value?.ToString();

            if (foundUser.Properties["employeeNumber"].Value != null)
            {
                if (uint.TryParse(foundUser.Properties["employeeNumber"].Value.ToString(), out uint result))
                {
                    Matrikelnummer = result;
                }
            }

            // Validation not needed, cause User exists
            IsValidated = true;
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 


        /// <summary>
        /// 
        /// </summary>
        private IdentificationTypes GetIdentType(string identString)
        {
            if (uint.TryParse(identString, out uint output))
            {
                return IdentificationTypes.Matrikelnummer;
            }

            if (identString.Contains("@"))
            {
                return IdentificationTypes.Email;
            }

            return IdentificationTypes.SamAccountName;
        }

    }
}
