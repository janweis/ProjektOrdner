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

        public AdUser User { get; set; }

        public PermissionRole AccessRole { get; set; }

        public PermissionSource Source { get; set; }
    }
}
