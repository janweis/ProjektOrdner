using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjektOrdner.Permission
{
    public class PermissionModel
    {
        public string ProjektPath { get; set; }

        public UserModel User { get; set; }

        public PermissionAccessRole AccessRole { get; set; }

        public PermissionSource Source { get; set; }
    }
}
