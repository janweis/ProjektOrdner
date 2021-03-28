using MimeKit;
using ProjektOrdner.App;
using ProjektOrdner.Mail;
using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public static class RepositoryUpdateService
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        public enum ServiceState { Running, Stopped };
        public static ServiceState State { get; set; }

        private static bool StopService { get; set; }
        private static bool IsInitializedSuccessfully { get; set; }
        private static AppSettings AppSettings { get; set; }
        private static IProgress<string> Progress { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 

        /// <summary>
        /// 
        /// Initialisierung...
        /// 
        /// </summary>
        public static bool Initialization(AppSettings appSettings, IProgress<string> progress)
        {
            StopService = false;
            State = ServiceState.Stopped;
            IsInitializedSuccessfully = false;
            AppSettings = appSettings;
            Progress = progress;

            if (null == appSettings)
                return false;

            if (null == progress)
                return false;





            IsInitializedSuccessfully = true;
            return true;
        }


        /// <summary>
        /// 
        /// Startet das Programm
        /// 
        /// </summary>
        public static async Task Run()
        {
            if (IsInitializedSuccessfully == false)
                throw new Exception("Is not successfully initalized!");

            TimeSpan waitForMe = new TimeSpan(0, 15, 0);
            int rootCounter = AppSettings.RootPaths.Count;

            do
            {
                for (int i = 0; i < rootCounter; i++)
                {
                    RepositoryRoot root = new RepositoryRoot(AppSettings.RootPaths[i], AppSettings);
                    Progress.Report($"[{(i + 1).ToString()}/{rootCounter.ToString()}] Verarbeite '{root.RootPath}'");

                    try
                    {
                        await ProcessRequestsAsync(root);
                        await UpdateRepositoryPermissionsAsync(root);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                await Task.Delay(waitForMe);
            } while (StopService == false);
        }


        /// <summary>
        /// 
        /// Stoppt den UpdateService
        /// 
        /// </summary>
        public static void Stop()
        {
            StopService = true;
            State = ServiceState.Stopped;
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 


        /// <summary>
        /// 
        /// Aktualisiert die Projektberechtigungen
        /// 
        /// </summary>
        private static async Task UpdateRepositoryPermissionsAsync(RepositoryRoot root)
        {
            Progress.Report("Aktualisiere die Projektberechtigungen ...");
            DirectoryInfo[] repositories = root.GetRepositories();

            foreach (DirectoryInfo repository in repositories)
            {
                PermissionProcessor permissionProcessor = new PermissionProcessor(repository.FullName, AppSettings);
                try
                {
                    await permissionProcessor.UpdatePermissionsAsync(null);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            Progress.Report("Aktualiserung der Projektberechtigungen abgeschlossen!");
        }


        /// <summary>
        /// 
        /// Verarbeitet die Projektanträge
        /// 
        /// </summary>
        private static async Task ProcessRequestsAsync(RepositoryRoot root)
        {
            RepositoryOrganization[] requests = await root.GetRepositoryRequestsAsync();

            if (null == requests || requests.Count() == 0)
                return;

            foreach (RepositoryOrganization organization in requests)
            {
                try
                {
                    RepositoryFolder repository = new RepositoryFolder(organization, new RepositorySettings(), RepositoryVersion.V2, AppSettings);
                    await repository.CreateAsync(Progress);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// 
        /// Create ProjektOrdner
        /// 
        /// </summary>
        private static async Task CreateProjekt(FileInfo file, IProgress<string> progress)
        {
            // Read Antragfile
            progress.Report($"Verarbeite den Projektantrag...");
            RepositoryOrganization repositoryOrga = new RepositoryOrganization();
            await repositoryOrga.LoadV1(file);
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
                foreach (RepositoryPermission permission in repositoryOrga.LegacyPermissions)
                {
                    // Add Permissions
                    await permission.AddToRepositoryAsync(repository);
                    //permTasks.Add(permissionProcessor.AddPermissionAsync(PermissionSource.File, permission));

                    // Create Mail
                    MailTemplateCreator templateCreator = new MailTemplateCreator(permission.User, repository.Organization);
                    string mailbody = string.Empty;
                    string mailbetreff = string.Empty;

                    switch (permission.Role)
                    {
                        case PermissionRole.ReadOnly:
                        {
                            mailbetreff = "Willkommen im neuen Projekt!";
                            mailbody = templateCreator.PermissionUserAdded(permission.Role);
                            break;
                        }
                        case PermissionRole.ReadWrite:
                        {
                            mailbetreff = "Willkommen im neuen Projekt!";
                            mailbody = templateCreator.PermissionUserAdded(permission.Role);
                            break;
                        }
                        case PermissionRole.Manager:
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