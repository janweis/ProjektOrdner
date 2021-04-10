using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class ManageRepositorysForm : Form
    {
        private AppSettings AppSettings { get; set; }
        private RepositoryFolder[] Repositories { get; set; }
        private TreeViewHelper TreeHelper { get; set; }
        private string CurrentRootPath { get; set; }
        private bool IncludeCorruptedProjects { get; set; }

        private IProgress<string> ProgressMessage { get; set; }
        private IProgress<int> RepoCounter { get; set; }


        public ManageRepositorysForm(AppSettings appSettings, RepositoryFolder[] repositories)
        {
            InitializeComponent();
            if (null == repositories)
                Repositories = new RepositoryFolder[0];
            else
                Repositories = repositories;

            AppSettings = appSettings;
            CurrentRootPath = AppSettings.RootPathDefault;
            IncludeCorruptedProjects = false;

            TreeHelper = new TreeViewHelper(ProjektsTree, ContextMenu2);
            appSettings.RootPaths.ForEach(path =>
            {
                if (path == appSettings.RootPathDefault)
                {
                    AddRootMenuItem(path, true);
                }
                else
                {
                    AddRootMenuItem(path, false);
                }
            });

            ProgressMessage = new Progress<string>(message => UpdateToolStripStatus(message));
            RepoCounter = new Progress<int>(count => UpdateRepositoryCounter(count));

            UpdateTreeView();
            UpdateRepositoryCounter();
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Functions
        // 


        /// <summary>
        /// 
        /// Aktiviert oder Deaktiviert die Mehrfachauswahl
        /// 
        /// </summary>
        private void SetMultiValueTree()
        {
            ToolStripMenuItem menuItem = mehrfachAuswahlModusToolStripMenuItem;

            if (menuItem.Checked == true)
            {
                // Control TreeView
                ProjektsTree.CheckBoxes = false;

                // Control MenuItem
                menuItem.Checked = false;
                umbenennenToolStripMenuItem.Enabled = true;
                umbenennenToolStripMenuItem1.Enabled = true;
            }
            else
            {
                // Control TreeView
                ProjektsTree.CheckBoxes = true;

                // Control MenuItem
                menuItem.Checked = true;
                umbenennenToolStripMenuItem.Enabled = false;
                umbenennenToolStripMenuItem1.Enabled = false;
            }

            ProgressMessage.Report("Ansichtsänderungen durchgeführt!");
        }


        /// <summary>
        /// 
        /// Legt ein Projekt an.
        /// 
        /// </summary>
        private async Task CreateProjektAsync()
        {
            // Add Projekt
            RepositoryAssistant repositoryManager = new RepositoryAssistant(CurrentRootPath, AppSettings);
            try
            {
                bool successfully = await repositoryManager.CreateRepositoryAsync(ProgressMessage);

                if (successfully)
                {
                    await UpdateRepositoryListAsync();
                    UpdateTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Das Repository konnte nicht angelegt werden!\n{ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ProgressMessage.Report("Projektanlage abgeschlossen!");
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task UpdateRepositoryListAsync()
        {
            RepositoryRoot root = new RepositoryRoot(CurrentRootPath, AppSettings);
            Repositories = await root.GetRepositoriesAsync(IncludeCorruptedProjects, ProgressMessage);
        }


        /// <summary>
        /// 
        /// Benennt ein Projekt um.
        /// 
        /// </summary>
        private async Task RenameProjektAsync()
        {
            TreeNode node = TreeHelper.GetNodeBySelection();
            if (null == node)
            {
                MessageBox.Show("Es wurde kein Projekt zum umbenennen ausgewählt!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RepositoryAssistant repositoryManager = new RepositoryAssistant(CurrentRootPath, AppSettings);
            try
            {
                bool successfully = await repositoryManager.RenameRepositoryAsync(node.Tag.ToString(), ProgressMessage);

                if (successfully)
                {
                    await UpdateRepositoryListAsync();
                    UpdateTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler beim umbenennen aufgetreten. {ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ProgressMessage.Report("Projekt umbenennen abgeschlossen!");
        }


        /// <summary>
        /// 
        /// Entfernt ein Projekt
        /// 
        /// </summary>
        private async Task RemoveProjektAsync()
        {
            // Nodes abrufen
            List<TreeNode> nodes = new List<TreeNode>();
            if (ProjektsTree.CheckBoxes == true)
            {
                List<TreeNode> nodeList = TreeHelper.GetNodesByChecked();
                if (null != nodeList)
                    nodes.AddRange(nodeList);
            }
            else
            {
                TreeNode selectedNode = TreeHelper.GetNodeBySelection();
                if (null != selectedNode)
                    nodes.Add(selectedNode);
            }

            if (null == nodes || nodes.Count == 0)
            {
                MessageBox.Show("Es wurde kein Projekt zum entfernen ausgewählt!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (TreeNode node in nodes)
            {
                try
                {
                    RepositoryAssistant repositoryManager = new RepositoryAssistant(CurrentRootPath, AppSettings);
                    await repositoryManager.RemoveRespositoryAsync(node.Tag.ToString(), ProgressMessage);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Es ist ein Fehler beim entfernen aufgetreten. {ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Update View
            await UpdateRepositoryListAsync();
            UpdateTreeView();
        }


        /// <summary>
        /// 
        /// Aktualisiert die Projektberechtigungen
        /// 
        /// </summary>
        private async Task UpdateRepositoryPermission()
        {
            TreeNode node = TreeHelper.GetNodeBySelection(true);

            if (null == node)
            {
                MessageBox.Show("Es wurde kein Projekt zum Aktualisieren ausgewählt!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Aktualisiere das aktuelle Projektt
            try
            {
                PermissionProcessor permissionProcessor = new PermissionProcessor(node.Tag.ToString(), AppSettings);
                await permissionProcessor.SyncPermissionsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten! {ex.Message}", "Update-Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ProgressMessage.Report("Berechtigungen wurden synchronisiert!");
        }


        /// <summary>
        /// 
        /// Aktualisiert alle Projektberechtigungen
        /// 
        /// </summary>
        private async Task UpdateRepositoriesPermissions()
        {
            ProgressMessage.Report("Update Projektberechtigungen aller Projekte...");

            // Aktualisiere für alle Projekte die Berechtigungen
            List<Task> tasks = new List<Task>();
            foreach (RepositoryFolder repository in Repositories)
            {
                if (repository.Status == RepositoryFolder.RepositoryStatus.Corrupted)
                    continue;

                PermissionProcessor permissionProcessor = new PermissionProcessor(repository.Organization.ProjektPath, AppSettings);
                tasks.Add(
                    permissionProcessor.SyncPermissionsAsync());
            }

            await Task.WhenAll(tasks);
            ProgressMessage.Report("Berechtigungen wurden synchronisiert!");
        }


        /// <summary>
        /// 
        /// Aktualisiert die Process Message anzeige
        /// 
        /// </summary>
        private void UpdateToolStripStatus(string message)
        {
            ToolStripProjektStatusMessage.Text = message;
            ToolStripProjektStatusMessage.Invalidate();
        }


        /// <summary>
        /// 
        /// Verarbeitet die sich verändernden Berechtigungen.
        /// 
        /// </summary>
        private async Task EditRepositoryPermission()
        {
            TreeNode selectedNode = TreeHelper.GetNodeBySelection();
            if (null == selectedNode)
            {
                // No Projekt selected!
                MessageBox.Show("Bitte wählen Sie zuerst ein Projekt aus!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                return;
            }

            // Sete den MasterNode, falls ein Child ausgewählt wurde!
            if (null != selectedNode.Parent)
                selectedNode = selectedNode.Parent;

            RepositoryFolder repository = TreeHelper.GetRepositoryFromNode(selectedNode, Repositories);
            if (null == repository)
                return;

            if (repository.Status == RepositoryFolder.RepositoryStatus.Corrupted)
            {
                // Projekt corrupted!
                MessageBox.Show("Berechtigungen eines defekten Projektes können nicht bearbeitet werden!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                return;
            }

            try
            {
                PermissionManager permissionManager = new PermissionManager(repository.Organization.ProjektPath, AppSettings);
                await permissionManager.ManagePermissions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten. {ex.Message}", "Berechtigungen bearbeiten", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ProgressMessage.Report("Berechtigungen wurden angepasst!");
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void OpenProjektFile()
        {
            string filePath = "";
            TreeNode node = TreeHelper.GetNodeBySelection();

            if (null == node || null == node.Parent)
                return; // Node ist kein Sub-Node!

            string projektPath = node.Parent.Tag.ToString();
            RepositoryOrganization organization = new RepositoryOrganization();
            RepositoryVersion repositoryVersion = organization.GetRepositoryVersion(projektPath);

            if (repositoryVersion == RepositoryVersion.Unknown)
            {
                MessageBox.Show("Unknown Repository Version! Could not detect Version!", "Open Permissionfile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (node.Name == "Manager" || node.Name == "Member" || node.Name == "Guest")
            {
                PermissionRole role = (PermissionRole)Enum.Parse(typeof(PermissionRole), node.Name);
                PermissionProcessor permissionProcessor = new PermissionProcessor(projektPath, AppSettings);
                filePath = permissionProcessor.GetPermissionFilePath(role);
            }
            else if (node.Name == "Settings")
            {
                filePath = RepositorySettings.GetSettingsFilePath(projektPath);
            }
            else if (node.Name == "Organization")
            {
                filePath = RepositoryOrganization.GetOrganizationFilePath(projektPath, repositoryVersion);
            }

            // Öffne das Programm
            Process process = new Process();
            process.StartInfo.FileName = filePath;
            try
            {
                process.Start();
                ProgressMessage.Report($"Die Datei {filePath} wurde geöffnet!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Die Datei konnte nicht geöffnet werden. {ex.Message}.\n\nPfad: {process.StartInfo.FileName}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }



        /// <summary>
        /// 
        /// 
        /// </summary>
        private async Task ProcessFilterCorruptedProjectsShownAsync()
        {
            if (IncludeCorruptedProjects == true)
            {
                IncludeCorruptedProjects = false;
                zeigeDefekteProjekteToolStripMenuItem.Checked = false;

            }
            else
            {
                IncludeCorruptedProjects = true;
                zeigeDefekteProjekteToolStripMenuItem.Checked = true;
            }

            await UpdateRepositoryListAsync();
            UpdateTreeView();

            ProgressMessage.Report("Neue Ansicht übernommen!");
        }


        /// <summary>
        /// 
        /// Aktualisiert die Ansicht
        /// 
        /// </summary>
        private void UpdateTreeView()
        {
            UpdateRepositoryCounter();
            TreeHelper.UpdateView(Repositories, IncludeCorruptedProjects);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateRepositoryCounter(int count = -1)
        {
            if (count != -1)
            {
                toolStripStatusProjektZahl.Text = $"{count.ToString()} Projekt(e)";
            }
            else
            {
                // Update Projektanzahl Anzeige
                if (null == Repositories)
                    toolStripStatusProjektZahl.Text = $"0 Projekt(e)";
                else
                    toolStripStatusProjektZahl.Text = $"{Repositories.Count()} Projekt(e)";
            }
        }


        /// <summary>
        /// 
        /// Fügt ein Menüeintrag der Stammverzeichnisse hinzu
        /// 
        /// </summary>
        private void AddRootMenuItem(string rootPath, bool isCurrentRootPath = false)
        {
            var tempMenuName = rootPath.Split('\\');
            var menuName = $@"{tempMenuName[tempMenuName.Length - 1]}\{tempMenuName[tempMenuName.Length - 2]}";

            ToolStripMenuItem menuRootItem = new ToolStripMenuItem(menuName)
            {
                Tag = rootPath,
                ToolTipText = rootPath,
                Checked = isCurrentRootPath
            };
            menuRootItem.Click += MenuRootItem_Click;

            // Add Menu
            projektRootToolStripMenuItem.DropDownItems.Add(menuRootItem);
        }


        /// <summary>
        /// 
        /// Ändert den Rootpfad und läd diese
        /// 
        /// </summary>
        private async Task ChangeRootPath(string rootPath)
        {
            CurrentRootPath = rootPath;         // Set new root Path

            await UpdateRepositoryListAsync();  // Update projekt list
            UpdateTreeView();                   // Update View

            ProgressMessage.Report("Stammverzeichnis gewechselt!");
        }


        /// <summary>
        /// 
        /// Start Update Service
        /// 
        /// </summary>
        private async Task StartUpdateServiceAsync()
        {
            // Set Menu Controls
            beendenToolStripMenuItem1.Enabled = true;
            startenToolStripMenuItem.Enabled = false;

            try
            {
                bool init = RepositoryUpdateService.Initialization(AppSettings, ProgressMessage);

                if (init)
                {
                    await RepositoryUpdateService.Run();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler im UpdateService aufgetreten! {ex.Message}");
                StopUpdateService();
            }
        }


        /// <summary>
        /// 
        /// Beendet Update Service
        /// 
        /// </summary>
        private void StopUpdateService()
        {
            // Set Menu Controls
            startenToolStripMenuItem.Enabled = true;
            beendenToolStripMenuItem1.Enabled = false;

            if (RepositoryUpdateService.State == RepositoryUpdateService.ServiceState.Running)
            {
                RepositoryUpdateService.Stop();
            }

        }


        /// <summary>
        /// 
        /// Zeigt die Ausgabe des Update Service
        /// 
        /// </summary>
        private void ShowServiceOutput()
        {
            try
            {
                RepositoryUpdateService.ShowResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten! {ex.Message}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task EnableFilter()
        {
            // Controls
            FilterToolStripButton.Checked = true;
            FilterToolStripButton.Enabled = false;

            // View
            try
            {
                await TreeHelper.SetFilterViewAsync(textBox1.Text, Repositories, RepoCounter, ProgressMessage, AppSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler in der Filterung! {ex.Message}", "Filter einschalten", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                await DisableFilter();
            }
            finally
            {
                // Controls
                FilterToolStripButton.Enabled = true;
                ProgressMessage.Report("Filter eingeschaltet!");
            }

        }


        /// <summary>
        /// 
        /// </summary>
        private async Task DisableFilter()
        {
            // Controls
            FilterToolStripButton.Checked = false;
            FilterToolStripButton.Enabled = false;

            // View
            try
            {
                await TreeHelper.SetFilterViewAsync("", Repositories, RepoCounter, ProgressMessage, AppSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler in der Filterung! {ex.Message}", "Filter ausschalten", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                // Controls
                FilterToolStripButton.Enabled = true;
                ProgressMessage.Report("Filter abgeschaltet!");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task ClickFilterAsync()
        {
            if (FilterToolStripButton.Checked)
                await EnableFilter();
            else
                await DisableFilter();
        }



        private async Task ExpandProjekt()
        {
            RepositoryFolder currentProjekt = TreeHelper.GetRepositoryFromSelectedNode(Repositories);
            if (null == currentProjekt)
            {
                MessageBox.Show("Das Projekt konnte nicht gefunden werden. Bitte wählen Sie ein Projekt aus.", "Verlängerung", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            ExpandProjektExpirationForm expandProjekt = new ExpandProjektExpirationForm(currentProjekt.Organization.ProjektName, currentProjekt.Organization.ProjektEnde);
            DialogResult result = expandProjekt.ShowDialog();

            if (result == DialogResult.OK)
            {
                await currentProjekt.Organization.ExpandExpireDate(currentProjekt.Organization.ProjektPath, expandProjekt.NewExpireDate);
            }

            ProgressMessage.Report("Projektlaufzeit wurde angepasst!");
        }




        //
        // Controls
        // ---------------------------------------------------------------------------------

        private async void MenuRootItem_Click(object sender, EventArgs e)
        {
            // Select new Root
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            try
            {
                await ChangeRootPath(menuItem.Tag.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten! {ex.Message}", "Stammpfad auswahl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Clear Checked
            for (int i = 1; i <= (projektRootToolStripMenuItem.DropDownItems.Count - 1); i++)
            {
                if (projektRootToolStripMenuItem.DropDownItems[i].GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem selectedMenuItem = projektRootToolStripMenuItem.DropDownItems[i] as ToolStripMenuItem;

                    if (selectedMenuItem.Checked == true)
                        selectedMenuItem.Checked = false;
                }
            }

            menuItem.Checked = true;
        }

        private void ProjektsTree_DoubleClick(object sender, EventArgs e) =>
            OpenProjektFile();

        private void button1_Click(object sender, EventArgs e) =>
            textBox1.Text = "";

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e) =>
            Close();

        private void MehrfachAuswahlModusToolStripMenuItem_Click(object sender, EventArgs e) =>
            SetMultiValueTree();

        private async void umbenennenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await RenameProjektAsync();

        private async void umbenennenToolStripMenuItem1_Click(object sender, EventArgs e) =>
            await RenameProjektAsync();

        private void EigenschaftenItem_Click(object sender, EventArgs e) =>
            TreeHelper.OpenProjektFolder();

        private void infoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ManagerInfoForm info = new ManagerInfoForm();
            info.ShowDialog();
        }

        private void aktualisierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateTreeView();
            ProgressMessage.Report("Aktualisierung abgeschlossen!");
        }

        private async void verwaltenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await RepositoryRoot.StartRootAssistantAsync(AppSettings, ProgressMessage);

        private async void anlegenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await CreateProjektAsync();

        private async void entfernenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await RemoveProjektAsync();

        private async void EntfernenItem_Click(object sender, EventArgs e) =>
            await RemoveProjektAsync();

        private async void HinzufügenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await EditRepositoryPermission();

        private void AusklappenItem_Click(object sender, EventArgs e) =>
            ProjektsTree.ExpandAll();

        private void EinklappenItem_Click(object sender, EventArgs e) =>
            ProjektsTree.CollapseAll();

        private async void UpdatePermissionsItem_Click(object sender, EventArgs e) =>
            await UpdateRepositoryPermission();

        private async void updateToolStripMenuItem_Click(object sender, EventArgs e) =>
            await UpdateRepositoryPermission();

        private async void aktualisiereAlleBerechtigungenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await UpdateRepositoriesPermissions();

        private async void ManagePermissionsMenuItem_Click(object sender, EventArgs e) =>
            await EditRepositoryPermission();

        private async void zeigeDefekteProjekteToolStripMenuItem_Click(object sender, EventArgs e) =>
            await ProcessFilterCorruptedProjectsShownAsync();

        private async void projekteErneutEinlesenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UpdateRepositoryListAsync();
            UpdateTreeView();
        }

        private async void startenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await StartUpdateServiceAsync();

        private void beendenToolStripMenuItem1_Click(object sender, EventArgs e) =>
            StopUpdateService();

        private void ausgabeEinblendenToolStripMenuItem_Click(object sender, EventArgs e) =>
            ShowServiceOutput();

        private async void AnlegenToolStripButton_Click(object sender, EventArgs e) =>
            await CreateProjektAsync();

        private async void EntfernenToolStripButton_Click(object sender, EventArgs e) =>
            await RemoveProjektAsync();

        private void abgelaufeneProjekteToolStripMenuItem2_Click(object sender, EventArgs e) =>
            textBox1.Text = "filter=expired";

        private void benutzerInProjektenToolStripMenuItem2_Click(object sender, EventArgs e) =>
            textBox1.Text = "filter=person=ExampleUser";

        private async void FilterToolStripButton_Click(object sender, EventArgs e) =>
            await ClickFilterAsync();

        private async void verlängernToolStripMenuItem_Click(object sender, EventArgs e) =>
            await ExpandProjekt();

    }
}
