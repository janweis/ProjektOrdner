using MimeKit;
using ProjektOrdner.Mail;
using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public class UpdateProjektPermissionsAsync
    {
        private string RootPath { get; set; }
        private AppSettings AppSettings { get; set; }
        private string PickupFolderPath { get; set; }


        public UpdateProjektPermissionsAsync(string rootPath, AppSettings appSettings)
        {
            RootPath = rootPath;
            AppSettings = appSettings;
        }

        /// <summary>
        /// 
        /// Startet das Programm
        /// 
        /// </summary>
        public async Task Run(IProgress<string> progress)
        {
            progress.Report($"Verarbeite '{RootPath}'");

            await ProcessRequests(progress);
            UpdateProjektPermissions(progress);
        }

        /// <summary>
        /// 
        /// Aktualisiert die Projektberechtigungen
        /// 
        /// </summary>
        private void UpdateProjektPermissions(IProgress<string> progress)
        {
            progress.Report("Aktualisiere die Projektberechtigungen ...");

            RepositoryRoot repositoryRoot = new RepositoryRoot(RootPath, AppSettings);
            DirectoryInfo[] repositories = repositoryRoot.GetRepositories();

            Array.ForEach(repositories, async repository =>
            {
                PermissionProcessor permissionProcessor = new PermissionProcessor(repository.FullName, AppSettings);
                await permissionProcessor.UpdatePermissionsAsync(null);
            });

            progress.Report("Aktualiserung der Projektberechtigungen abgeschlossen!");
        }

        /// <summary>
        /// 
        /// Verarbeitet die Projektanträge
        /// 
        /// </summary>
        private async Task ProcessRequests(IProgress<string> progress)
        {
            // Check PickupFolder
            string pickupFolderName = "_Projekt beantragen";
            PickupFolderPath = Path.Combine(RootPath, pickupFolderName);

            if (Directory.Exists(PickupFolderPath) == false)
            {
                progress.Report($"Der Pickup Ordner unter '{PickupFolderPath}' wurde nicht gefunden!");
                return;
            }

            progress.Report($"Prüfe auf Anträge ...");

            // Get Files
            string[] requestFiles = Directory.GetFiles(PickupFolderPath);

            IEnumerable<FileInfo> validRequestFiles = null;
            if (null == requestFiles)
            {
                return;
            }
            else
            {
                validRequestFiles = requestFiles
                    .Select(file => new FileInfo(file))
                    .Where(file => file.Name.StartsWith("_") == false)
                    .Where(file => file.Extension == ".txt");

                if (null == validRequestFiles || validRequestFiles.Count() == 0)
                {
                    return;
                }
            }


            // Process Files
            var foundJobFiles = validRequestFiles.GetEnumerator();
            while (foundJobFiles.MoveNext())
            {
                FileInfo selectedFile = foundJobFiles.Current;

                try
                {
                    progress.Report($"Antrag gefunden: {selectedFile.FullName}");
                    await CreateProjekt(selectedFile, progress);
                }
                catch (Exception ex)
                {
                    progress.Report($"Fehler: {ex.Message}");

                    // Rename defekt file
                    selectedFile.MoveTo($@"{selectedFile.Directory.FullName}\_fehler_{selectedFile.Name}");
                }
            }
        }


        /// <summary>
        /// 
        /// Create ProjektOrdner
        /// 
        /// </summary>
        private async Task CreateProjekt(FileInfo file, IProgress<string> progress)
        {
            // Read Antragfile
            progress.Report($"Verarbeite den Projektantrag...");
            RepositoryOrganization repositoryOrga = new RepositoryOrganization();
            await repositoryOrga.LoadV1(file, RootPath);
            bool isFileValid = repositoryOrga.IsValid();

            // Validate Antragfile
            if (isFileValid == false)
            {
                throw new Exception($"Der Antrag ist ungültig!; {file.FullName}");
            }

            // Validate Directorypath
            if (Directory.Exists(repositoryOrga.ProjektPath) == true)
            {
                throw new Exception("Das Projekt existiert bereits!");
            }

            // Create Repository
            progress.Report($"Erstelle den ProjektOrdner...");

            RepositoryFolder repositoryProcessor = new RepositoryFolder(AppSettings);
            RepositoryFolder repository = new RepositoryFolder(repositoryOrga, new RepositorySettings(), RepositoryVersion.V2, AppSettings);

            await repositoryProcessor
                .CreateAsync(repository, progress);

            // Setup Projekt Permissions
            if (repositoryOrga.LegacyPermissions.Count > 0)
            {
                progress.Report("Mitglieder werden berechtigt ...");

                PermissionProcessor permissionProcessor = new PermissionProcessor(repositoryOrga.ProjektPath, AppSettings);
                MailProcessor mailProcessor = new MailProcessor(AppSettings, repository);

                List<Task> permTasks = new List<Task>();
                List<MimeMessage> mails = new List<MimeMessage>();
                foreach (PermissionModel permission in repositoryOrga.LegacyPermissions)
                {
                    // Add Permissions
                    permTasks.Add(permissionProcessor.AddPermissionAsync(PermissionSource.File, permission));

                    // Create Mail
                    MailTemplateCreator templateCreator = new MailTemplateCreator(permission.User, repository.Organization);
                    string mailbody = string.Empty;
                    string mailbetreff = string.Empty;

                    switch (permission.AccessRole)
                    {
                        case PermissionAccessRole.ReadOnly:
                        {
                            mailbetreff = "Willkommen im neuen Projekt!";
                            mailbody = templateCreator.PermissionUserAdded(permission.AccessRole);
                            break;
                        }
                        case PermissionAccessRole.ReadWrite:
                        {
                            mailbetreff = "Willkommen im neuen Projekt!";
                            mailbody = templateCreator.PermissionUserAdded(permission.AccessRole);
                            break;
                        }
                        case PermissionAccessRole.Manager:
                        {
                            mailbetreff = "ProjektOrdner wurde angelegt!";
                            mailbody = templateCreator.ProjektCreatedTemplate();
                            break;
                        }
                    }

                    // Craete and add Mail to List
                    mails.Add(mailProcessor.CreateMail(mailbody, mailbetreff, permission.User));

                    await Task.WhenAll(permTasks.ToArray());
                }
                progress.Report("Berechtigungen abgeschlossen!");

                // Inform via Mail
                progress.Report("Sende Benachrichtigungen via Mail...");
                mails.ForEach(mail =>
                {
                    try
                    {
                        //mailProcessor.SendMail(mail);
                    }
                    catch (Exception ex)
                    {
                        progress.Report($"Fehler: {ex.Message}");
                    }
                });

                // Cleanup
                progress.Report("Cleanup ...");
                File.Delete(file.FullName);

                // ENDE
                progress.Report("ProjektOrdner erfolgreich erstellt!");
            }
        }
    }
}