using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Mail
{
    internal class MailTemplateCreator
    {
        private UserModel User { get; set; }
        private RepositoryOrganization RepositoryOrga { get; set; }

        public MailTemplateCreator(UserModel user, RepositoryOrganization repositoryOrga)
        {
            User = user;
            RepositoryOrga = repositoryOrga;
        }

        /// <summary>
        /// 
        /// Mail > ProjektManager
        /// 
        /// </summary>
        public string ProjektCreatedTemplate()
        {
            return $@"{BegrüßungBlock()}

es wurde ein neues Projekt angelegt, indem Sie als ProjektManager angegeben wurden.

{ProjektPfadeBlock()}

{ProjektOrgaDataBlock(PermissionAccessRole.Manager)}
{ProjektInformationenBlock()}

{AbschlussBlock()}";
        }
        
        /// <summary>
        /// 
        /// Mail > ProjektManager
        /// 
        /// </summary>
        public string ProjektRemovedTemplate()
        {
            return $@"{BegrüßungBlock()}

der ProjektOrdner {RepositoryOrga.Name}, auf dem Sie ProjektManager waren, wurde gelöscht. Ein weiterer Zugriff ist nicht möglich.
{ProjektOrgaDataBlock(PermissionAccessRole.Manager)}

{AbschlussBlock()}";
        }

        /// <summary>
        /// 
        /// Mail > ProjetManager
        /// 
        /// </summary>
        public string ProjektReminderTemplate(int days)
        {
            return $@"{BegrüßungBlock()}

der ProjektOrdner {RepositoryOrga.Name}, in dem Sie ProjektManager sind, wird in {days.ToString()} um 22 Uhr gelöscht. Bitte kümmern Sie sich frühzeitig um ein Backup Ihrer Projektdaten. Projektverlängerungen können Sie in der IT-Abteilung beantragen.
{ProjektOrgaDataBlock(PermissionAccessRole.Manager)}

{AbschlussBlock()}";
        }

        /// <summary>
        /// 
        /// Mail > Mitglied
        /// 
        /// </summary>
        public string PermissionUserAdded(PermissionAccessRole accessRole)
        {
            return $@"{BegrüßungBlock()}

willkommen im ProjektOrdner '{RepositoryOrga.Name}'. Sie wurden durch einen ProjektManager zu dem Projekt hinzugefügt.

{ProjektBerechtigungBlock(accessRole)}
{ProjektPfadeBlock()}

{ProjektInformationenBlock()}

{AbschlussBlock()}";
        }

        /// <summary>
        /// 
        /// Mail > Mitglied
        /// 
        /// </summary>
        public string PermissionUserRemoved()
        {
            return $@"{BegrüßungBlock()}

der Zugriff auf das Projekt '{RepositoryOrga.Name}' wurde Ihnen entzogen. Sie sind nun kein Mitglied in diesem Projekt mehr.

{ProjektPfadeBlock()}

{AbschlussBlock()}";
        }

        /// <summary>
        /// 
        /// Mail > ProjektManager
        /// 
        /// </summary>
        public string PermissionChangeReportTemplate(PermissionModel[] addedUsers, PermissionModel[] removedUsers)
        {
            StringBuilder report = new StringBuilder();

            // Report Benutzer hinzugefügt
            int i = 1;
            if (null != addedUsers)
            {
                i = 1;
                report.AppendLine("## Benutzer hinzugefügt:");
                foreach (PermissionModel permission in addedUsers)
                {
                    report.AppendLine($"\t{i.ToString()}) {permission.User.Vorname} {permission.User.Nachname}; {permission.AccessRole.ToString()}");
                    i++;
                }
            }

            // Report Benutzer entfernt
            if (null != removedUsers)
            {
                if (null != addedUsers)
                    report.AppendLine(); // Trennlinie, falls Benutzer auch hinzugefügt wurden

                i = 1;
                report.AppendLine("## Benutzer entfernt:");
                foreach (PermissionModel permission in removedUsers)
                {
                    report.AppendLine($"\t{i.ToString()}) {permission.User.Vorname} {permission.User.Nachname}");
                    i++;
                }
            }

            return $@"{BegrüßungBlock()}

diese Mail informiert Sie über die Berechtigungsanpassungen an einem ProjektOrdner. Folgend(e) aufgelistete(n) Änderung(en) wurde(n) am Projekt {RepositoryOrga.Name} durchgeführt.

{report.ToString()}

{ProjektPfadeBlock()}

{AbschlussBlock()}";
        }



        //
        // Generisch
        // 

        private string ProjektBerechtigungBlock(PermissionAccessRole accessRole)
        {
            return $"Ihre Berechtigungen: {accessRole.ToString()}";
        }

        private string ProjektPfadeBlock()
        {
            return $@"Folgende Zugriffsmöglichkeiten stehen Ihnen zur Verfügung:
    (1) 'I:\02_IFM Projekte\{RepositoryOrga.Name}'
    (2) '\\ifmsrvfile.institut-ifm.de\data\02_IFM Projekte\{RepositoryOrga.Name}'";
        }

        private string ProjektOrgaDataBlock(PermissionAccessRole accessRole)
        {
            return $@"## ProjektOrdner - Organisation:
    Name: {RepositoryOrga.Name}
    Ablaufdatum: {RepositoryOrga.EndeDatum.ToLongDateString()}
    Ihre Berechtigung: {accessRole.ToString()}";
        }

        private string ProjektInformationenBlock()
        {
            return @"## ProjektOrdner - Wichtige Informationen:
    (1) Es werden 10GB Speicher zur Verfügung gestellt.
    (2) Berechtigungen können jederzeit selbst im Projekt angepasst werden.
    (3) Wichtig: Falls Sie auf den ProjektOrdner nicht zugreifen können, müssen Sie sich vermutlich erneut am Gerät anmelden.";
        }

        private string BegrüßungBlock()
        {
            return $"Sehr geehrte(r) {User.Vorname} {User.Nachname},\n";
        }

        private string AbschlussBlock()
        {
            return "Mit freundlicher Unterstützung\nIhre IFM IT-Abteilung\n\n Diese Mail wurde automatisch generiert.";
        }

    }
}
