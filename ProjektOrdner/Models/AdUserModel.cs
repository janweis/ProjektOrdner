using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Models
{
    public class AdUserModel
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string SamAccountName { get; set; }
        public string Email { get; set; }
        public string Matrikelnummer { get; set; }
        public string UserPrincipalName { get; set; }

        public AdUserModel(UserPrincipal user)
        {
            if (null == user)
                throw new ArgumentNullException();

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
                        Matrikelnummer = directoryEntry.Properties["employeeNumber"].Value.ToString();
                    }
                }
            }
        }
    }
}
