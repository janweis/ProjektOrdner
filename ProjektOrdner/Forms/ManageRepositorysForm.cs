using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Processors;
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
        private RepositoryModel[] Repositorys { get; set; }
        private ManagerNodeProcessor NodeProcessor { get; set; }
        private string CurrentRootPath { get; set; }
        private bool IncludeCorruptedProjects { get; set; }

        public ManageRepositorysForm(AppSettings appSettings, RepositoryModel[] repositorys)
        {
            InitializeComponent();
            Repositorys = repositorys;
            AppSettings = appSettings;
            CurrentRootPath = AppSettings.RootPathDefault;
            IncludeCorruptedProjects = false;

            NodeProcessor = new ManagerNodeProcessor(ProjektsTree, ContextMenu2);

            UpdateViewAsync(
                new Progress<string>(message => UpdateToolStripStatus(message)), repositorys);
        }

        //
        // FUNCTIONS
        // _________________________________________________________________________________

        /// <summary>
        /// 
        /// Ruft ein Repository anhand des Names in einem Node ab
        /// 
        /// </summary>
        private RepositoryModel GetRepositoryFromNode(TreeNode node)
        {
            if (null == node)
                return null;

            return Repositorys
                .Where(repo => node.Name == repo.RepositoryOrga.ProjektName)
                .FirstOrDefault();
        }


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
        /// Ruft alle Projekte ab.
        /// 
        /// </summary>
        private async Task<RepositoryModel[]> GetProjectsAsync(IProgress<string> progressMessage, bool includeCorrupted = false)
        {
            RepositoryProcessor repositoryProcessor = new RepositoryProcessor(AppSettings);
            return await repositoryProcessor.GetRepositorysAsync(CurrentRootPath, progressMessage, includeCorrupted);
        }


        /// <summary>
        /// 
        /// Legt ein Projekt an.
        /// 
        /// </summary>
        private async Task CreateProjektAsync()
        {
            // Message routing
            var messageLabel = new Progress<string>(message => UpdateToolStripStatus(message));

            // Add Projekt
            try
            {
                RepositoryManager repositoryManager = new RepositoryManager(CurrentRootPath, AppSettings);
                await repositoryManager.CreateRepositoryAsync(messageLabel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Konnte das Projekt nicht anlegen!\n{ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update View
            await UpdateViewAsync(messageLabel);
        }


        /// <summary>
        /// 
        /// Benennt ein Projekt um.
        /// 
        /// </summary>
        private async Task RenameProjektAsync()
        {
            // Message routing
            var messageLabel = new Progress<string>(message => UpdateToolStripStatus(message));

            TreeNode node = NodeProcessor.GetNodeBySelection();
            if (null == node)
            {
                MessageBox.Show("Es wurde kein Projekt zum umbenennen ausgewählt!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RepositoryManager repositoryManager = new RepositoryManager(CurrentRootPath, AppSettings);

            try
            {
                await repositoryManager.RenameRepositoryAsync(node.Tag.ToString(), messageLabel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler beim umbenennen aufgetreten. {ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Update View
            await UpdateViewAsync(messageLabel);
        }


        /// <summary>
        /// 
        /// Entfernt ein Projekt
        /// 
        /// </summary>
        private async Task RemoveProjektAsync()
        {
            // Message routing
            var messageLabel = new Progress<string>(message => UpdateToolStripStatus(message));

            // Nodes abrufen
            List<TreeNode> nodes = new List<TreeNode>();
            if (ProjektsTree.CheckBoxes == true)
            {
                List<TreeNode> nodeList = NodeProcessor.GetNodesByCheckBoxes();
                if (null != nodeList)
                    nodes.AddRange(nodeList);
            }
            else
            {
                TreeNode selectedNode = NodeProcessor.GetNodeBySelection();
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
                    await repositoryManager.RemoveRespositoryAsync(node.Tag.ToString(), messageLabel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Es ist ein Fehler beim entfernen aufgetreten. {ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Update View
            await UpdateViewAsync(messageLabel);
        }


        /// <summary>
        /// 
        /// Aktualisiert die Projektberechtigungen
        /// 
        /// </summary>
        private async Task UpdateProjektPermission()
        {
            TreeNode node = NodeProcessor.GetNodeBySelection(true);

            if (null == node)
            {
                MessageBox.Show("Es wurde kein Projekt zum entfernen ausgewählt!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Aktualisiere das aktuelle Projekt
            PermissionProcessor permissionProcessor = new PermissionProcessor(node.Tag.ToString(), AppSettings);
            await permissionProcessor.UpdatePermissionsAsync(null);
        }


        /// <summary>
        /// 
        /// Aktualisiert alle Projektberechtigungen
        /// 
        /// </summary>
        private async Task UpdateProjektsPermissions(IProgress<string> progress)
        {
            List<Task> tasks = new List<Task>();
            progress.Report("Update Projektberechtigungen aller Projekte...");

            // Aktualisiere für alle Projekte die Berechtigungen
            foreach (RepositoryModel repository in Repositorys)
            {
                if (repository.Status == RepositoryModel.RepositoryStatus.Corrupted)
                    continue;

                PermissionProcessor permissionProcessor = new PermissionProcessor(repository.RepositoryOrga.ProjektPath, AppSettings);
                tasks.Add(
                    permissionProcessor.UpdatePermissionsAsync(null));
            }

            await Task.WhenAll(tasks);
            progress.Report("Berechtigungsaktualisierung abgeschlossen!");
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
            TreeNode selectedNode = NodeProcessor.GetNodeBySelection();
            if (null == selectedNode)
            {
                // No Projekt selected!
                MessageBox.Show("Bitte wählen Sie zuerst ein Projekt aus!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                return;
            }

            // Sete den MasterNode, falls ein Child ausgewählt wurde!
            if (null != selectedNode.Parent)
                selectedNode = selectedNode.Parent;

            RepositoryModel repository = GetRepositoryFromNode(selectedNode);
            if (repository.Status == RepositoryModel.RepositoryStatus.Corrupted)
            {
                // Projekt corrupted!
                MessageBox.Show("Berechtigungen eines defekten Projektes können nicht bearbeitet werden!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                return;
            }

            try
            {
                PermissionManager permissionManager = new PermissionManager(repository.RepositoryOrga.ProjektPath, AppSettings);
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
            TreeNode node = NodeProcessor.GetNodeBySelection();

            if (null == node || null == node.Parent)
                return; // Node ist kein Sub-Node!

            RepositoryModel repository = Repositorys
                .Where(repo => repo.RepositoryOrga.ProjektName == node.Parent.Text)
                .FirstOrDefault();

            if (null == repository)
                return; // Kein zugehöriges Projekt gefunden!

            // Get Filename
            string fileName = "";
            if (null != node.Parent)
            {
                switch (node.Name)
                {
                    case "Manager":
                    {
                        fileName = AppConstants.PermissionFileManagerName;
                        break;
                    }

                    case "Change":
                    {
                        fileName = AppConstants.PermissionFileReadWriteName;
                        break;
                    }

                    case "Read":
                    {
                        fileName = AppConstants.PermissionFileReadOnlyName;
                        break;
                    }

                    case "Settings":
                    {
                        fileName = AppConstants.RepositorySettingsFileName;
                        break;
                    }

                    case "Organization":
                    {
                        if (repository.Version == RepositoryVersion.V1)
                        {
                            fileName = AppConstants.OrganisationFileNameV1;
                        }
                        else if (repository.Version == RepositoryVersion.V2)
                        {
                            fileName = AppConstants.OrganisationFileNameV2;
                        }

                        break;
                    }
                }
            }

            // Definiere den Projektpfad der Datei
            Process process = new Process();
            if (repository.Version == RepositoryVersion.V1)
            {
                process.StartInfo.FileName = $@"{node.Parent.Tag.ToString()}\{fileName}";
            }
            else if (repository.Version == RepositoryVersion.V2)
            {
                process.StartInfo.FileName = $@"{node.Parent.Tag.ToString()}\{AppConstants.OrganisationFolderName}\{fileName}";
            }

            // Öffne das Programm
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
            NodeProcessor.SetFilterView(textBox1.Text, Repositorys);
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

            await UpdateViewAsync(
                new Progress<string>(message => UpdateToolStripStatus(message)));
        }


        /// <summary>
        /// 
        /// Aktualisiert die Ansicht
        /// 
        /// </summary>
        private async Task UpdateViewAsync(IProgress<string> message)
        {
            // Update Nodes
            Repositorys = await GetProjectsAsync(message, IncludeCorruptedProjects);
            NodeProcessor.UpdateView(Repositorys, IncludeCorruptedProjects);  // Update Nodes

            // Update Projektanzahl Anzeige
            toolStripStatusProjektZahl.Text = $"{Repositorys.Count()} Projekt(e)";

            // Update Status
            message.Report("Aktualisierung abgeschlossen!");
        }


        /// <summary>
        /// 
        /// Aktualisiert die Ansicht
        /// 
        /// </summary>
        private void UpdateViewAsync(IProgress<string> message, RepositoryModel[] repositories)
        {
            // Update Nodes
            NodeProcessor.UpdateView(repositories, IncludeCorruptedProjects);

            // Update Projektanzahl Anzeige
            toolStripStatusProjektZahl.Text = $"{Repositorys.Count()} Projekt(e)";

            // Update Status
            message.Report("Aktualisierung abgeschlossen!");
        }



        //
        // Controls
        // ---------------------------------------------------------------------------------

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
            NodeProcessor.OpenProjektFolder();

        private void infoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ManagerInfoForm info = new ManagerInfoForm();
            info.ShowDialog();
        }

        private void aktualisierenToolStripMenuItem_Click(object sender, EventArgs e) =>
            UpdateViewAsync(new Progress<string>(message => UpdateToolStripStatus(message)), Repositorys);

        private async void verwaltenToolStripMenuItem_Click(object sender, EventArgs e) => 
            await AppSettings.ManageRootPathsAsync();

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
            await UpdateProjektsPermissions(new Progress<string>(message => UpdateToolStripStatus(message)));

        private async void ManagePermissionsMenuItem_Click(object sender, EventArgs e) =>
            await ManageProjektPermissionAsync();

        private async void zeigeDefekteProjekteToolStripMenuItem_Click(object sender, EventArgs e) =>
            await ProcessFilterCorruptedProjectsShownAsync();

        private async void projekteErneutEinlesenToolStripMenuItem_Click(object sender, EventArgs e) =>
            await UpdateViewAsync(new Progress<string>(message => UpdateToolStripStatus(message)));
    }
}
