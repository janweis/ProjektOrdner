using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public class RepositorySettingsModel
    {
        // Bei Berechtigungsänderungen werden die ProjektManager über die Änderung nach der Umsetzung informiert.
        // Standard: SendMailByPermissionReadWrites = 1 
        public int MailByPermissionReadWrites { get; set; } = 1;

        // Die Projektmitglieder werden informiert, wenn Sie dem Projekt beitreten
        // Standard: SendMailByJoining = 1
        public int MailByJoining { get; set; } = 1;

        // Die Projektmitglieder werden informiert, wenn Sie das Projekt verlassen
        // Standard: SendMailByLeaving = 1
        public int MailByLeaving { get; set; } = 1;

        // Über die restliche Laufzeit des ProjektOrdners informieren
        // Standard: SendMailByExpiring = 1
        public int MailByExpiring { get; set; } = 1;

        // Einzelne Mitglieder vom Mailemfpang ausnehmen
        // Standard: MailReceipientExclution = 
        public string[] MailRecipientExclution { get; set; } = new string[0];

        // ProjektOrdner Version
        public string SettingsVersion { get; set; } = "1.0.0.0";


        // Sonderfunktion
        public bool ProjektVorEndeDerLaufzeitEntfernen { get; set; } = false;
        public bool SindSieSichSicher { get; set; } = false;


        public RepositorySettingsModel() { }
    }
}
