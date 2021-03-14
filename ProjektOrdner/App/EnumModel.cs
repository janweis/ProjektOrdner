using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public enum PermissionSource { ActiveDirectory, File }

    public enum PermissionAccessRole
    {
        [Description("Nur Lesen")]
        ReadOnly,

        [Description("Lesen & Schreiben")]
        ReadWrite,

        [Description("ProjektManager")]
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
