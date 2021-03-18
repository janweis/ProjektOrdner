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
        private AppSettings Settings { get; set; }

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
            SplashScreenForm splashScreen = new SplashScreenForm();
            splashScreen.FormClosed += ApplicationExitAction;
            splashScreen.Show();

            // Definde Progress 
            Progress<int> numberProgress = new Progress<int>(percent => { splashScreen.UpdatePercent(percent); });
            Progress<string> messageProgress = new Progress<string>(projekt => { splashScreen.UpdateProjekt(projekt); });

            // Wait...
            WriteProgress("Starte mit den Vorbeitungen ...", messageProgress);
            await Task.Delay(1500);

            // Load Application Settings
            WriteProgress("Lade Settings...", messageProgress);
            await Task.Delay(10);

            try
            {
                Settings = new AppSettings();
                await Settings.LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
            }


            // Load Projects
            WriteProgress("Lade Projekte ...", messageProgress);
            await Task.Delay(10);

            RepositoryFolder[] repositories = null;
            if (null != Settings?.RootPathDefault)
            {
                RepositoryRoot repositoryRoot = new RepositoryRoot(Settings?.RootPathDefault, Settings);
                repositories = await repositoryRoot.GetRepositories(false, messageProgress);
            }
            else
            {
                MessageBox.Show("Es wurde kein Stammverzeichnis für Projekte gefunden. Bitte legen Sie eins an.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RepositoryRoot.StartRootAssistant(Settings, messageProgress);
            }

            // Start ManagerUI
            ManageRepositorysForm manageProjekts = new ManageRepositorysForm(Settings, repositories);
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
