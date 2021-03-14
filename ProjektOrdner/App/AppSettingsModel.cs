using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public class AppSettingsModel
    {
        //
        // Active Directory Einstellungen
        //

        // Active Directory Domänen Name
        public string AdDomainName { get; set; } = Environment.UserDomainName;

        // Dort werden die AD-BenutzerModel ausgelesen
        // Distinguished Name wird benötigt
        public string AdUserRootDN { get; set; }

        // Dort werden die GlobalGroup AD-Gruppen angelegt
        // Distinguished Name wird benötigt
        public string AdGroupGgDN { get; set; }

        // Dort werden die DomainLocalGroup AD-Gruppen angelegt
        // Distinguished Name wird benötigt
        public string AdGroupDlDN { get; set; }

        // Prefix 
        public string AdGroupNamePrefix { get; set; }

        // AD-Gruppen Bezeichnung für das Thema
        public string AdGroupNameTopic { get; set; }

        // Suffix für den lesenen Zugriff Gruppennamen
        public string AdGroupNameSuffixRead { get; set; }

        // Suffix für den schreibenden Zugriff Gruppennamen
        public string AdGroupNameSuffixWrite { get; set; }

        // Suffix für den manager Zugriff Gruppennamen
        public string AdGroupNameSuffixManager { get; set; }


        public string AdGroupScopeGlobalName { get; set; }
        public string AdGroupScopeLocalName { get; set; }


        // Fortlaufende GruppenID für künftige AD-Gruppen
        public int AdGroupID { get; set; } = 0;


        //
        // Dateisystem Einstellungen
        //


        // Dort werden die Root-Ordner gespeichert
        public AppSettingsRootProfileProcessor RootFolders { get; set; }


        // Dort wird die Protokollierung eingeschaltet
        public bool Logging { get; set; } = false;

        // Protokollpfad
        public string LogPath { get; set; } = "";


        //
        // BenutzerModel limitierungen
        //

        public int UserMaxProjektOrdner { get; set; } = 0;
        public string UserDeniedCreateProjekts { get; set; }


        //
        // Mail System
        //

        // VON
        public string MailFrom { get; set; }

        // Ein- und Ausschalter für das senden von Info-Mails
        public int MailDisabled { get; set; } = 0;

        // Angabe des FDQN des Mailservers
        public string MailServer { get; set; }

        // Angabe des Mailports für das Versenden von Mails
        public int MailPort { get; set; } = 25;


        // Konstruktor
        public AppSettingsModel() { }


    }
}
