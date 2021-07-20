using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ProjektOrdner.Permission.PermissionNodeProcessor;

namespace ProjektOrdner.Forms
{
    public partial class ManagePermissionsForm : Form
    {
        public List<RepositoryPermission> Permissions { get; set; }
        private PermissionNodeProcessor NodeProcessor { get; set; }
        private AppSettings AppSettings { get; set; }

        private bool IsMultiValue = false;
        private bool NodeAdvancedMode = false;

        public ManagePermissionsForm(List<RepositoryPermission> permissions, AppSettings appSettings)
        {
            InitializeComponent();

            AppSettings = appSettings;
            Permissions = permissions;
            NodeProcessor = new PermissionNodeProcessor(PermissionTreeView);

            // Inits
            InitializeControls();
        }


        //
        // FUNCTIONS
        // ______________________________________________________________________________________________


        /// <summary>
        /// 
        /// Initialise Controls
        /// 
        /// </summary>
        private void InitializeControls()
        {
            NodeProcessor.ResetView();
            NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);
        }


        /// <summary>
        /// 
        /// Aktiviert oder Deaktiviert die Mehrfachauswahl
        /// 
        /// </summary>
        private void SetMultiValueTree()
        {
            if (MehrfachauswahlCheckBox.Checked == true)
            {
                // Control TreeView
                PermissionTreeView.CheckBoxes = true;
                IsMultiValue = true;
            }
            else
            {
                // Control TreeView
                PermissionTreeView.CheckBoxes = false;
                IsMultiValue = false;
            }
        }


        /// <summary>
        /// 
        /// Sucht das verknüfte Objekt aus der Berechtigungsliste heraus.
        /// 
        /// </summary>
        private RepositoryPermission GetPermissionFromNode(TreeNode treeNode)
        {
            if (null == treeNode)
                return null;

            return Permissions
                .Where(permission => $"{permission.User.Nachname}, {permission.User.Vorname}" == treeNode.Name)
                .FirstOrDefault();
        }


        /// <summary>
        /// 
        /// Add User
        /// 
        /// </summary>
        private void AddUserToView()
        {
            List<AdUser> adUsers = new List<AdUser>();
            FindAdUserForm finderForm = new FindAdUserForm(AppSettings);
            DialogResult dialogResult;

            try
            {
                dialogResult = finderForm.ShowDialog();
                adUsers = finderForm.AdUserResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten! {ex.Message}");
                return;
            }

            if (dialogResult == DialogResult.Cancel)
                return;

            // Prepare
            NodeProcessor.ClearMasterNode(MasterNode.Unbestimmt);
            PermissionTreeView.BeginUpdate();

            foreach (AdUser adUser in adUsers)
            {
                // Process Nodes
                if (NodeAdvancedMode == true)
                {
                    // Advanced Nodes
                    NodeProcessor.AddNodeAdvanced(adUser, MasterNode.Unbestimmt);
                }
                else
                {
                    // Simple Nodes
                    NodeProcessor.AddNodeSimple(adUser, MasterNode.Unbestimmt);
                }

                Permissions.Add(new RepositoryPermission(adUser, PermissionRole.Undefined, PermissionSource.File, AppSettings));
            }

            // Finish
            PermissionTreeView.EndUpdate();
        }


        /// <summary>
        /// 
        /// Remove User
        /// 
        /// </summary>
        private void RemoveSelectedUser()
        {
            if (IsMultiValue)
            {
                // Multi User selected
                List<TreeNode> selectedNodes = NodeProcessor.GetNodesByCheckBoxes(true);
                if (null == selectedNodes)
                    return;

                foreach(TreeNode node in selectedNodes)
                {
                    RepositoryPermission foundPermission = GetPermissionFromNode(node);
                    if(null != foundPermission)
                        Permissions.Remove(foundPermission);
                }
            }
            else
            {
                // Single User selected

                TreeNode selectedNode = NodeProcessor.GetNodeBySelection();
                if (null == selectedNode)
                    return; // nichts ausgewählt

                RepositoryPermission foundPermission = GetPermissionFromNode(selectedNode);
                if (null == foundPermission)
                    return; // keine Zuordnung gefunden

                Permissions.Remove(foundPermission);
            }

            NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);
        }


        /// <summary>
        /// 
        /// Das Objekt im TreeView wird eine Berechtigungsstufe höher gesetzt.
        /// 
        /// </summary>
        private void MoveEntry(bool up)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();

            if (IsMultiValue)
            {
                treeNodes = NodeProcessor.GetNodesByCheckBoxes(true);
            }
            else
            {
                TreeNode treeNode = NodeProcessor.GetNodeBySelection();
                if (null != treeNode)
                    treeNodes.Add(treeNode);
            }

            foreach (TreeNode treeNode in treeNodes)
            {
                if (null == treeNode && null == treeNode.Parent)
                    return; // nichts ausgewählt, oder kein VaterObjekt vorhanden

                var foundPermission = GetPermissionFromNode(treeNode);
                if (null == foundPermission)
                    continue; // keine Zuordnung gefunden

                switch (treeNode.Parent.Index)
                {
                    case 0:
                    {
                        if (up == false)
                        {
                            NodeProcessor.MoveNodeTo(treeNode, MasterNode.Manager);
                            foundPermission.Role = PermissionRole.Manager;
                        }

                        break;
                    }
                    case 1:
                    {
                        if (up)
                        {
                            NodeProcessor.MoveNodeTo(treeNode, MasterNode.Unbestimmt);
                            foundPermission.Role = PermissionRole.Undefined;
                        }
                        else
                        {
                            NodeProcessor.MoveNodeTo(treeNode, MasterNode.Mitarbeiter);
                            foundPermission.Role = PermissionRole.Member;
                        }

                        break;
                    }
                    case 2:
                    {
                        if (up)
                        {
                            NodeProcessor.MoveNodeTo(treeNode, MasterNode.Manager);
                            foundPermission.Role = PermissionRole.Manager;
                        }
                        else
                        {
                            NodeProcessor.MoveNodeTo(treeNode, MasterNode.NurLesen);
                            foundPermission.Role = PermissionRole.Guest;
                        }

                        break;
                    }
                    case 3:
                    {
                        if (up)
                        {
                            NodeProcessor.MoveNodeTo(treeNode, MasterNode.Mitarbeiter);
                            foundPermission.Role = PermissionRole.Member;
                        }

                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// Schmeißt die ungültigen Berechtigungen aus der Berechtigungsliste und schließt das Fenster.
        /// 
        /// </summary>
        private void ApplyPermissions()
        {
            Permissions.RemoveAll(permission => permission.Role == PermissionRole.Undefined);
            Close();
        }



        //
        // CONTROLS
        // _____________________________________________________________________________________________


        private void SimpleViewRadio_CheckedChanged(object sender, EventArgs e)
        {
            NodeAdvancedMode = false;
            NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);
        }

        private void AdvancedViewRadio_CheckedChanged(object sender, EventArgs e)
        {
            NodeAdvancedMode = true;
            NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);
        }

        private void AbbrechenButton_Click(object sender, EventArgs e) =>
            Close();

        private void ÜbernehmenButton_Click(object sender, EventArgs e) =>
            ApplyPermissions();

        private void ResetButton_Click(object sender, EventArgs e) =>
            NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);

        private void AddUserButton_Click(object sender, EventArgs e) =>
            AddUserToView();

        private void EntfernenButton_Click(object sender, EventArgs e) =>
            RemoveSelectedUser();

        private void SetUpButton_Click(object sender, EventArgs e) =>
            MoveEntry(true);

        private void SetDownButton_Click(object sender, EventArgs e) =>
            MoveEntry(false);

        private void MehrfachauswahlCheckBox_CheckedChanged(object sender, EventArgs e) =>
            SetMultiValueTree();
    }
}
