using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Permission
{
    public static class PermissionFileTemplate
    {
        public static string GetPermissionTemplate(PermissionRole permissionAccess)
        {
            string accessType = "";
            switch (permissionAccess)
            {
                case PermissionRole.Guest:
                {
                    accessType = "Nur Lesen";
                    break;
                }
                case PermissionRole.Member:
                {
                    accessType = "Lesen & Schreiben";
                    break;
                }
                case PermissionRole.Manager:
                {
                    accessType = "Manager";
                    break;
                }
            }

            string content = $@"
# # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
# 
# ProjektOrdner-Berechtigungsdatei - {accessType}
# 
# <> WICHTIG <>
# o Verwenden Sie nur den Windows-Anmeldename, Matrikelnummer oder die Emailadresse, um den Benutzer zu berechtigen.
# o Berechtigungen werden automatisiert halbstündlich :15 - :45 übernommen und Sie via Mail benachrichtigt.
# o Verändern Sie den Namen der Datei nicht.
##
# <> ANLEITUNGEN <>
# Die Anleitungen sind unter '_ProjektOrdner beantragen' zu finden.
#
# <> BEISPIELE <>
# Mein.Benutzername
# Mein.Benutzername@emailadresse.de
#
";
            return content;
        }


    }
}
