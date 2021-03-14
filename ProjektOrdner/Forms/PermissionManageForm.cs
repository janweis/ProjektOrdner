using ProjektOrdner.App;
using ProjektOrdner.Permission;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ProjektOrdner.Permission.PermissionNodeProcessor;

namespace ProjektOrdner.Forms
{
    public partial class PermissionManageForm : Form
    {
        public List<PermissionModel> Permissions { get; set; }
        private PermissionNodeProcessor NodeProcessor { get; set; }
        private AppSettingsModel AppSettings { get; set; }

        private bool NodeAdvancedMode = false;

        public PermissionManageForm(List<PermissionModel> permissions, AppSettingsModel appSettings)
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
        /// Sucht das verknüfte Objekt aus der Berechtigungsliste heraus.
        /// 
        /// </summary>

        private PermissionModel GetPermissionFromNode(TreeNode treeNode)
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
            List<UserModel> adUsers = new List<UserModel>();

            ActiveDirectoryUserFinderForm finderForm = new ActiveDirectoryUserFinderForm(adUsers, AppSettings);
            DialogResult dialogResult;

            try
            {
                dialogResult = finderForm.ShowDialog();
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

            foreach (UserModel adUser in adUsers)
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

                Permissions.Add(new PermissionModel()
                {
                    User = adUser,
                    AccessRole = PermissionAccessRole.Undefined,
                    Source = PermissionSource.File
                });
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
            TreeNode selectedNode = NodeProcessor.GetNodeBySelection();
            if (null == selectedNode)
                return; // nichts ausgewählt

            var foundPermission = GetPermissionFromNode(selectedNode);
            if (null == foundPermission)
                return; // keine Zuordnung gefunden

            Permissions.Remove(foundPermission);
            NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);
        }


        /// <summary>
        /// 
        /// Das Objekt im TreeView wird eine Berechtigungsstufe höher gesetzt.
        /// 
        /// </summary>

        private void MoveEntryUp()
        {
            TreeNode selectedNode = NodeProcessor.GetNodeBySelection();
            if (null == selectedNode && null == selectedNode.Parent)
                return; // nichts ausgewählt, oder kein VaterObjekt vorhanden

            var foundPermission = GetPermissionFromNode(selectedNode);
            if (null == foundPermission)
                return; // keine Zuordnung gefunden

            switch (selectedNode.Parent.Index)
            {
                case 1:
                {
                    NodeProcessor.MoveNodeTo(selectedNode, MasterNode.Unbestimmt);
                    foundPermission.AccessRole = PermissionAccessRole.Undefined;
                    break;
                }
                case 2:
                {
                    NodeProcessor.MoveNodeTo(selectedNode, MasterNode.Manager);
                    foundPermission.AccessRole = PermissionAccessRole.Manager;
                    break;
                }
                case 3:
                {
                    NodeProcessor.MoveNodeTo(selectedNode, MasterNode.Mitarbeiter);
                    foundPermission.AccessRole = PermissionAccessRole.ReadWrite;
                    break;
                }
            }

            PermissionTreeView.Select();
        }


        /// <summary>
        /// 
        /// Das Objekt im TreeView wird eine Berechtigungsstufe tiefer gesetzt.
        /// 
        /// </summary>

        private void MoveEntryDown()
        {
            TreeNode selectedNode = NodeProcessor.GetNodeBySelection();
            if (null == selectedNode)
                return; // nichts ausgewählt

            var foundPermission = GetPermissionFromNode(selectedNode);
            if (null == foundPermission)
                return; // keine Zuordnung gefunden

            switch (selectedNode.Parent.Index)
            {
                case 0:
                {
                    NodeProcessor.MoveNodeTo(selectedNode, MasterNode.Manager);
                    foundPermission.AccessRole = PermissionAccessRole.Manager;
                    break;
                }
                case 1:
                {
                    NodeProcessor.MoveNodeTo(selectedNode, MasterNode.Mitarbeiter);
                    foundPermission.AccessRole = PermissionAccessRole.ReadWrite;
                    break;
                }
                case 2:
                {
                    NodeProcessor.MoveNodeTo(selectedNode, MasterNode.NurLesen);
                    foundPermission.AccessRole = PermissionAccessRole.ReadOnly;
                    break;
                }
            }

            PermissionTreeView.Select();
        }


        /// <summary>
        /// 
        /// Schmeißt die ungültigen Berechtigungen aus der Berechtigungsliste und schließt das Fenster.
        /// 
        /// </summary>

        private void ApplyPermissions()
        {
            // Aktualisiere Berechtigungen
            Permissions = Permissions
                .Where(permission => permission.AccessRole != PermissionAccessRole.Undefined)
                .ToList();

            Close();
        }



        //
        // CONTROLS
        // ______________________________________________________________________________________________


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

        private void AbbrechenButton_Click(object sender, EventArgs e) => Close();

        private void ÜbernehmenButton_Click(object sender, EventArgs e) => ApplyPermissions();

        private void ResetButton_Click(object sender, EventArgs e) => NodeProcessor.UpdateView(Permissions.ToArray(), NodeAdvancedMode);

        private void AddUserButton_Click(object sender, EventArgs e) => AddUserToView();

        private void EntfernenButton_Click(object sender, EventArgs e) => RemoveSelectedUser();

        private void SetUpButton_Click(object sender, EventArgs e) => MoveEntryUp();

        private void SetDownButton_Click(object sender, EventArgs e) => MoveEntryDown();
    }
}
