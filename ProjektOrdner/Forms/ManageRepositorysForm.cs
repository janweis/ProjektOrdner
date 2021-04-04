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
        private IProgress<string> Progress { get; set; }

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

            Progress = new Progress<string>(message => UpdateToolStripStatus(message));
            UpdateViewAsync(repositories);
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
        }


        /// <summary>
        /// 
        /// Legt ein Projekt an.
        /// 
        /// </summary>
        private async Task CreateProjektAsync()
        {
            // Add Projekt
            RepositoryManager repositoryManager = new RepositoryManager(CurrentRootPath, AppSettings);
            try
            {
                await repositoryManager.CreateRepositoryAsync(Progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Konnte das Projekt nicht anlegen!\n{ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update View
            UpdateViewAsync();
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

            RepositoryManager repositoryManager = new RepositoryManager(CurrentRootPath, AppSettings);

            try
            {
                await repositoryManager.RenameRepositoryAsync(node.Tag.ToString(), Progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler beim umbenennen aufgetreten. {ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Update View
            UpdateViewAsync();
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
                    RepositoryManager repositoryManager = new RepositoryManager(CurrentRootPath, AppSettings);
                    await repositoryManager.RemoveRespositoryAsync(node.Tag.ToString(), Progress);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Es ist ein Fehler beim entfernen aufgetreten. {ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Update View
            UpdateViewAsync();
        }


        /// <summary>
        /// 
        /// Aktualisiert die Projektberechtigungen
        /// 
        /// </summary>
        private async Task UpdateProjektPermission()
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
        }


        /// <summary>
        /// 
        /// Aktualisiert alle Projektberechtigungen
        /// 
        /// </summary>
        private async Task UpdateProjektsPermissions()
        {
            List<Task> tasks = new List<Task>();
            Progress.Report("Update Projektberechtigungen aller Projekte...");

            // Aktualisiere für alle Projekte die Berechtigungen
            foreach (RepositoryFolder repository in Repositories)
            {
                if (repository.Status == RepositoryFolder.RepositoryStatus.Corrupted)
                    continue;

                PermissionProcessor permissionProcessor = new PermissionProcessor(repository.Organization.ProjektPath, AppSettings);
                tasks.Add(
                    permissionProcessor.SyncPermissionsAsync());
            }

            await Task.WhenAll(tasks);
            Progress.Report("Berechtigungsaktualisierung abgeschlossen!");
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
        private async Task ManageProjektPermissionAsync()
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

            RepositoryFolder repository = TreeHelper.GetRepositoryFromNode(Repositories);
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
                MessageBox.Show($"Es ist ein Fehler aufgetreten.\n{ex.Message}");
                return;
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Die Datei konnte nicht geöffnet werden. {ex.Message}.\n\nPfad: {process.StartInfo.FileName}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void ProcessFilterButtonStatus()
        {
            TreeHelper.SetFilterView(textBox1.Text, Repositories);
            if (textBox1.Text.Length > 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }


        /// <summary>
        /// 
        /// 
        /// </summary>
        private void ProcessFilterCorruptedProjectsShownAsync()
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

            UpdateViewAsync();
        }


        /// <summary>
        /// 
        /// Aktualisiert die Ansicht
        /// 
        /// </summary>
        private void UpdateViewAsync()
        {
            // Update Nodes
            if (null == Repositories || Repositories.Count() == 0)
            {
                Progress.Report("Kein Projekt gefunden!");
                return;
            }

            TreeHelper.UpdateView(Repositories, IncludeCorruptedProjects);  // Update Nodes

            // Update Projektanzahl Anzeige
            toolStripStatusProjektZahl.Text = $"{Repositories.Count()} Projekt(e)";

            // Update Status
            Progress.Report("Aktualisierung abgeschlossen!");
        }


        /// <summary>
        /// 
        /// Aktualisiert die Ansicht
        /// 
        /// </summary>
        private void UpdateViewAsync(RepositoryFolder[] repositories)
        {
            if (null == repositories || repositories.Count() == 0)
            {
                Progress.Report("Kein Projekt gefunden!");
                return;
            }

            // Update Nodes
            TreeHelper.UpdateView(repositories, IncludeCorruptedProjects);

            // Update Projektanzahl Anzeige
            toolStripStatusProjektZahl.Text = $"{Repositories.Count()} Projekt(e)";

            // Update Status
            Progress.Report("Aktualisierung abgeschlossen!");
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
            CurrentRootPath = rootPath;
            Repositories = null;

            // Get Repositories
            RepositoryRoot root = new RepositoryRoot(CurrentRootPath, AppSettings);
            Repositories = await root.GetRepositoriesAsync(IncludeCorruptedProjects, Progress);

            // Update View
            UpdateViewAsync();
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
                bool init = RepositoryUpdateService.Initialization(AppSettings, Progress);

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

        private void textBox1_TextChanged(object sender, EventArgs e) =>
            ProcessFilterButtonStatus();

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

        private void aktualisierenToolStripMenuItem_Click(object sender, EventArgs e) =>
            UpdateViewAsync();

        private async void verwaltenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await RepositoryRoot.StartRootAssistantAsync(AppSettings, Progress);

        private async void anlegenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await CreateProjektAsync();

        private async void entfernenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await RemoveProjektAsync();

        private async void EntfernenItem_Click(object sender, EventArgs e) =>
            await RemoveProjektAsync();

        private async void HinzufügenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await ManageProjektPermissionAsync();

        private void AusklappenItem_Click(object sender, EventArgs e) =>
            ProjektsTree.ExpandAll();

        private void EinklappenItem_Click(object sender, EventArgs e) =>
            ProjektsTree.CollapseAll();

        private async void UpdatePermissionsItem_Click(object sender, EventArgs e) =>
            await UpdateProjektPermission();

        private async void updateToolStripMenuItem_Click(object sender, EventArgs e) =>
            await UpdateProjektPermission();

        private async void aktualisiereAlleBerechtigungenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await UpdateProjektsPermissions();

        private async void ManagePermissionsMenuItem_Click(object sender, EventArgs e) =>
            await ManageProjektPermissionAsync();

        private void zeigeDefekteProjekteToolStripMenuItem_Click(object sender, EventArgs e) =>
            ProcessFilterCorruptedProjectsShownAsync();

        private void projekteErneutEinlesenToolStripMenuItem_Click(object sender, EventArgs e) =>
            UpdateViewAsync();

        private async void startenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await StartUpdateServiceAsync();

        private void beendenToolStripMenuItem1_Click(object sender, EventArgs e) =>
            StopUpdateService();

        private void ausgabeEinblendenToolStripMenuItem_Click(object sender, EventArgs e) =>
            ShowServiceOutput();

    }
}
