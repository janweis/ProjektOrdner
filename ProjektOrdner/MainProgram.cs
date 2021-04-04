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
            IProgress<string> splashMessage = new Progress<string>(projekt => { splashScreen.UpdateProjekt(projekt); });

            // Load Application Settings
            splashMessage.Report("Lade Programmeinstellungen");
            await Task.Delay(10);

            try
            {
                Settings = new AppSettings();
                await Settings.LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Programmeinstellungen! {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
            }


            // Load Projects
            splashMessage.Report("Lade Projekte...");
            await Task.Delay(10);

            RepositoryFolder[] repositories = null;
            if (string.IsNullOrWhiteSpace(Settings.RootPathDefault) == false)
            {
                RepositoryRoot repositoryRoot = new RepositoryRoot(Settings?.RootPathDefault, Settings);
                repositories = await repositoryRoot.GetRepositoriesAsync(false, splashMessage);
            }
            else
            {
                MessageBox.Show("Es wurde kein Stammverzeichnis für Projekte gefunden. Bitte legen Sie eins an.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RepositoryRoot.StartRootAssistantAsync(Settings, splashMessage);
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
        /// Action zum Beenden der Form.
        /// 
        /// </summary>
        private void ApplicationExitAction(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
