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
                Settings = new AppSettings();
                await Settings.LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
            }

            // Validate Roots
            WriteProgress("Verifiziere Standard RootPath...", messageProgress);
            await Task.Delay(10);

            do
            {
                if (null == Settings.RootPathDefault)
                {
                    MessageBox.Show("Der default RootPath ist nicht definiert. Nachfolgend bitte die RootPaths einpflegen","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    await Settings.ManageRootPathsAsync();
                }
                else
                {
                    if (Directory.Exists(Settings.RootPathDefault) == true)
                        break;
                }
            } while (true);


            // Load Projects
            WriteProgress("Lade Projekte ...", messageProgress);
            await Task.Delay(10);

            RepositoryModel[] repositories = null;
            try
            {
                RepositoryProcessor repositoryProcessor = new RepositoryProcessor(Settings);
                repositories = await repositoryProcessor.GetRepositorysAsync((string)Settings?.RootPathDefault, messageProgress, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Die Projekte konnten nicht eingelesen werden. {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashScreen.Close();
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
