using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public class UserModel
    {
        public enum IdentificationTypes { SamAccountName, Email, Matrikelnummer }

        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string EMail { get; set; }
        public string SamAccountName { get; set; }
        public uint Matrikelnummer { get; set; }

        public IdentificationTypes IdentificationType { get; }
        public string Identification { get; set; }

        public bool IsValidated { get; set; }


        public UserModel(UserPrincipal user)
        {
            Vorname = user.GivenName;
            Nachname = user.Surname;
            EMail = user.EmailAddress;
            SamAccountName = user.SamAccountName;

            IdentificationType = IdentificationTypes.SamAccountName;
            Identification = SamAccountName;

            IsValidated = true;
        }

        public UserModel(string identString)
        {
            Identification = identString;
            IdentificationType = GetIdentType(Identification);

            switch (IdentificationType)
            {
                case IdentificationTypes.SamAccountName:
                    SamAccountName = identString;
                    break;

                case IdentificationTypes.Email:
                    EMail = identString;
                    break;

                case IdentificationTypes.Matrikelnummer:
                    Matrikelnummer = uint.Parse(identString);
                    break;
            }

            IsValidated = false;
        }


        //
        // Helper Functions
        // 

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
