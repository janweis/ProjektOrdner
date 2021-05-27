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
            Log.Write("Start Logging");

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

            State = ServiceState.Running;
            TimeSpan waitForMe = new TimeSpan(0, 1, 0);
            int rootCounter = AppSettings.RootPaths.Count;
            string stage = string.Empty;

            do
            {
                for (int i = 0; i < rootCounter; i++)
                {
                    RepositoryRoot root = new RepositoryRoot(AppSettings.RootPaths[i], AppSettings);
                    WriteProgress($"[{(i + 1).ToString()}/{rootCounter.ToString()}] Verarbeite '{root.RootPath}'");

                    try
                    {
                        stage = "Process Requests";
                        await ProcessRequestsAsync(root);

                        stage = "Sync Permissions";
                        await UpdateRepositoryPermissionsAsync(root);
                    }
                    catch (Exception ex)
                    {
                        WriteProgress($"Error in stage {stage}: {ex.Message}");
                    }
                }

                WriteProgress($"ServiceTask abgeschlossen! Warte eine Minute...");
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
            WriteProgress("ServiceTask wurde gestoppt!");
            StopService = true;
            State = ServiceState.Stopped;
            Log.Close();
        }


        /// <summary>
        /// 
        /// Öffnet das Log
        /// 
        /// </summary>
        public static void ShowResults() =>
            Log.ShowLog();


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
            DirectoryInfo[] repositories = root.GetRepositories();

            foreach (DirectoryInfo repository in repositories)
            {
                try
                {
                    PermissionProcessor permissionProcessor = new PermissionProcessor(repository.FullName, AppSettings);
                    await permissionProcessor.SyncPermissionsAsync();
                }
                catch (Exception ex)
                {
                    WriteProgress($"Es ist ein Fehler in der Synchronisierung der Berechtigungen für das Projekt {repository.FullName} aufgetreten! {ex.Message}");
                }
            }
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
                RepositoryProcessor repositoryProcessor = new RepositoryProcessor(root, AppSettings, Progress);
                await repositoryProcessor.CreateAsync(organization);

                if (null != organization.LegacyPermissions || organization.LegacyPermissions.Count() > 0)
                {
                    PermissionProcessor permissionProcessor = new PermissionProcessor(organization.ProjektPath, AppSettings);
                    await permissionProcessor.GrantPermissionsAsync(organization.LegacyPermissions.ToArray());
                }

                File.Delete(organization.RequestFilePath);
            }
        }


        /// <summary>
        /// 
        /// Write Messages
        /// 
        /// </summary>
        private static void WriteProgress(string message)
        {
            Log.Write(message);
            Progress.Report(message);
        }

    }
}