using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public class PermissionComparer : IEqualityComparer<RepositoryPermission>
    {
        public bool Equals(RepositoryPermission x, RepositoryPermission y)
        {

            if (x.User.SamAccountName.ToLower().Equals(y.User.SamAccountName.ToLower()) == true)
            {
                // Benutzername gleich

                if (x.Role == y.Role)
                {
                    // Berechtigung gleich
                    return true;
                }
            }

            return false;
        }

        public int GetHashCode(RepositoryPermission obj)
        {
            int hashSamAccountName = obj.User.SamAccountName.ToLower().GetHashCode();
            int hashAccess = obj.Role.GetHashCode();

            return (hashAccess + hashSamAccountName).GetHashCode();
        }
    }
}
