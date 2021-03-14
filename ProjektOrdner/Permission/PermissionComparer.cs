using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public class PermissionComparer : IEqualityComparer<PermissionModel>
    {
        public bool Equals(PermissionModel x, PermissionModel y)
        {

            if (x.User.SamAccountName.ToLower().Equals(y.User.SamAccountName.ToLower()) == true)
            {
                // Benutzername gleich

                if (x.AccessRole == y.AccessRole)
                {
                    // Berechtigung gleich
                    return true;
                }
            }

            return false;
        }

        public int GetHashCode(PermissionModel obj)
        {
            int hashSamAccountName = obj.User.SamAccountName.ToLower().GetHashCode();
            int hashAccess = obj.AccessRole.GetHashCode();

            return (hashAccess + hashSamAccountName).GetHashCode();
        }
    }
}
