using ProjektOrdner.App;
using ProjektOrdner.Permission;
using System;
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
        private static LogProcessor Log { get; set; }


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

            Log = new LogProcessor(appSettings.LogPath);

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


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public static void ShowResults()
        {
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
        /// Verarbeitet die Projektanträge und erstellt ggf. das Repository und die Berechtigung
        /// 
        /// </summary>
        private static async Task ProcessRequestsAsync(RepositoryRoot root)
        {
            RepositoryOrganization[] requests = await root.GetRepositoryRequestsAsync();

            if (null == requests || requests.Count() == 0)
                return;

            foreach (RepositoryOrganization organization in requests)
            {
                RepositoryFolder repository = new RepositoryFolder(organization, new RepositorySettings(), RepositoryVersion.V2, AppSettings);
                try
                {
                    await repository.CreateAsync(Progress);
                }
                catch (Exception)
                {
                    throw;
                }

                foreach (RepositoryPermission permission in organization.LegacyPermissions)
                {
                    try
                    {
                        await permission.AddToRepositoryAsync(repository);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }


    }
}