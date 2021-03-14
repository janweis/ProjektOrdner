using ProjektOrdner.App;
using ProjektOrdner.Forms;
using ProjektOrdner.Repository;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner
{
    public class MainProgram
    {
        private AppSettingsModel AppSettings { get; set; }

        public MainProgram() { }

        //
        // MAIN-FUNCTIONS
        // ______________________________________________________________________________________


        /// <summary>
        /// 
        /// Startet das Programm.
        /// 
        /// </summary>

        public async Task StartAsync()
        {
            // Load Splash
            AppSplashScreenForm splashScreen = new AppSplashScreenForm();
            splashScreen.FormClosed += ApplicationExitAction;
            splashScreen.Show();

            // Definde Progress 
            Progress<int> numberProgress = new Progress<int>(percent => { splashScreen.UpdatePercent(percent); });
            Progress<string> messageProgress = new Progress<string>(projekt => { splashScreen.UpdateProjekt(projekt); });

            // Wait...
            await Task.Delay(1500);

            // Load Application Settings
            WriteProgress("Lade Settings...", messageProgress);
            await Task.Delay(10);

            try
            {
                AppSettingsProcessor settingsProcessor = new AppSettingsProcessor();
                AppSettings = await settingsProcessor.ReadSettingsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
            }

            if (null == AppSettings)
            {
                MessageBox.Show($"Es ist ein Fehler beim Lesen der AppSettings.json aufgetreten!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
            }

            // Validate Roots
            WriteProgress("Verifiziere Rootpfade...", messageProgress);
            await Task.Delay(10);
            AppSettingsRootProfileModel rootFolderProfile = null;

            do
            {
                rootFolderProfile = AppSettings?.RootFolders?.GetDefaultRoot();

                if (null != rootFolderProfile)
                {
                    if (Directory.Exists(rootFolderProfile.Path) == true)
                    {
                        break;
                    }
                }

                DialogResult dialogResult = MessageBox.Show("Der ProjektOrdner RootPath ist nicht erreichbar oder exitiert nicht. Möchten Sie die Pfade kontrollieren?", "Hinweis", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.No)
                    break;

                // Manage Settings
                RootPathManager pathManager = new RootPathManager(AppSettings);
                await pathManager.ManageAsync();

                // Set new Settings
                AppSettings = pathManager.AppSettings;
            } while (true);


            // Load Projects
            WriteProgress("Lade Projekte...", messageProgress);
            await Task.Delay(10);

            RepositoryModel[] repositories = null;
            try
            {
                RepositoryProcessor repositoryProcessor = new RepositoryProcessor(AppSettings);
                repositories = await repositoryProcessor.GetRepositorysAsync(AppSettings?.RootFolders?.GetDefaultRoot().Path, messageProgress, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Die Projekte konnten nicht eingelesen werden. {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
            }

            // Start ManagerUI
            ManageRepositorysForm manageProjekts = new ManageRepositorysForm(AppSettings, repositories);
            manageProjekts.FormClosed += ApplicationExitAction;
            splashScreen.Owner = manageProjekts;
            manageProjekts.Show();

            // Close SplashScreen
            splashScreen.FormClosed -= ApplicationExitAction;
            splashScreen.Close();
        }


        //
        // SUB-FUNCTIONS
        // ______________________________________________________________________________________


        /// <summary>
        /// 
        /// Aktualisiert die SplashScreen ausgabe
        /// 
        /// </summary>
        private void WriteProgress(string message, IProgress<string> progress)
        {
            progress.Report(message);
        }


        /// <summary>
        /// 
        /// Action zum Beenden der Form.
        /// 
        /// </summary>
        private void ApplicationExitAction(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
