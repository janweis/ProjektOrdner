using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public enum PermissionSource { ActiveDirectory, File }

    public enum PermissionRole
    {
        [Description("Guest")]
        Guest,

        [Description("Member")]
        Member,

        [Description("Manager")]
        Manager,

        [Description("Unbestimmt")]
        Undefined
    }

    public enum RepositoryVersion
    {
        [Description("Version 1")]
        V1,

        [Description("Version 2")]
        V2,

        [Description("Unbekannt")]
        Unknown
    }

}
