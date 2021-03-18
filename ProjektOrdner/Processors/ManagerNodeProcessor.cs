using ProjektOrdner.App;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Processors
{
    public class ManagerNodeProcessor
    {
        private TreeView View { get; set; }
        private ContextMenuStrip Context { get; set; }

        public ManagerNodeProcessor(TreeView treeView, ContextMenuStrip contextMenuStrip)
        {
            View = treeView;
            Context = contextMenuStrip;
        }


        //
        // Get Node
        //

        public TreeNode GetNodeByName(string Name)
        {
            if (View.Nodes.ContainsKey(Name) == true)
            {
                return View.Nodes[Name];
            }

            return null;
        }

        public TreeNode GetNodeByID(string id)
        {
            foreach (TreeNode node in View.Nodes)
            {
                if (node.Tag.ToString() == id)
                {
                    return node;
                }
            }

            return null;
        }

        public TreeNode GetNodeBySelection(bool onlyMasterNode = false)
        {
            if(onlyMasterNode == false)
                return View.SelectedNode;

            if (null == View.SelectedNode.Parent)
            {
                return View.SelectedNode;
            }
            else
            {
                return View.SelectedNode.Parent;
            }
        }

        public List<TreeNode> GetNodesByCheckBoxes()
        {
            List<TreeNode> nodeList = null;

            if (View.CheckBoxes == true)
            {
                nodeList = new List<TreeNode>();

                // Get checked Projects
                foreach (TreeNode node in View.Nodes)
                {
                    if (node.Checked == true && null == node.Parent)
                        nodeList.Add(node);
                }
            }

            return nodeList;
        }


        //
        // Add Node
        //

        public void AddProjektNodeCorrupted(string Name, string projektPath)
        {
            // Add Projekt Node
            View.Nodes.Add(Name, Name, 5, 5);
            View.Nodes[Name].Tag = projektPath;
            View.Nodes[Name].NodeFont = new System.Drawing.Font(View.Font, System.Drawing.FontStyle.Italic);
        }

        public void AddProjektNodeV2(string Name, string projektPath)
        {
            // Add Projekt Node
            View.Nodes.Add(Name, Name, 6, 6);
            View.Nodes[Name].Tag = projektPath;

            // Add Attribute Nodes
            View.Nodes[Name].ContextMenuStrip = Context;
            View.Nodes[Name].Nodes.Add("Manager", "Berechtigungen - Manager", 1, 1);
            View.Nodes[Name].Nodes.Add("Change", "Berechtigungen - Lesen & Schreiben", 1, 1);
            View.Nodes[Name].Nodes.Add("Read", "Berechtigungen - Nur Lesen", 1, 1);
            View.Nodes[Name].Nodes.Add("Settings", "Einstellungen", 0, 0);
            View.Nodes[Name].Nodes.Add("Organization", "ProjektOrdner", 2, 2);
        }

        public void AddProjektNodeV1(string Name, string projektPath)
        {
            // Add Projekt Node
            View.Nodes.Add(Name, Name, 6, 6);
            View.Nodes[Name].Tag = projektPath;

            // Add Attribute Nodes
            View.Nodes[Name].ContextMenuStrip = Context;
            View.Nodes[Name].Nodes.Add("Change", "Berechtigungen - Lesen & Schreiben", 1, 1);
            View.Nodes[Name].Nodes.Add("Read", "Berechtigungen - Nur Lesen", 1, 1);
            View.Nodes[Name].Nodes.Add("Organization", "ProjektOrdner v1", 2, 2);
        }


        //
        // Remove Node
        // 

        public void RemoveProjektNodeByName(string Name)
        {
            TreeNode node = GetNodeByName(Name);

            if (null != node)
                View.Nodes.Remove(node);
        }

        public void RemoveProjektNodeByID(string id)
        {
            TreeNode node = GetNodeByID(id);

            if (null != node)
                View.Nodes.Remove(node);
        }


        //
        // Special Functions
        //

        /// <summary>
        /// 
        /// Aktualisiert die Projektansicht
        /// 
        /// </summary>

        public void UpdateView(RepositoryFolder[] repositories, bool includeCorrupted = false)
        {
            // Pepare
            View.BeginUpdate();
            View.Nodes.Clear();

            // Projekte zum View hinzufügen
            foreach (RepositoryFolder repository in repositories)
            {
                switch (repository.Status)
                {
                    case RepositoryFolder.RepositoryStatus.Ok:
                    {
                        switch (repository.Organization.Version)
                        {
                            case RepositoryVersion.V1:
                            {
                                AddProjektNodeV1(repository.Organization.ProjektName, repository.Organization.ProjektPath);
                                break;
                            }
                            case RepositoryVersion.V2:
                            {
                                AddProjektNodeV2(repository.Organization.ProjektName, repository.Organization.ProjektPath);
                                break;
                            }
                            case RepositoryVersion.Unknown:
                                break;
                        }

                        break;
                    }
                    case RepositoryFolder.RepositoryStatus.Corrupted:
                    {
                        if (includeCorrupted == true)
                            AddProjektNodeCorrupted(repository.Organization.ProjektName, repository.Organization.ProjektPath);

                        break;
                    }
                    case RepositoryFolder.RepositoryStatus.NotChecked:
                        break;
                }
            }

            // Cleanup and Update
            View.EndUpdate();
        }


        /// <summary>
        /// 
        /// Filtert die Projekt anzeige
        /// 
        /// </summary>

        public void SetFilterView(string filter, RepositoryFolder[] repositories)
        {
            IEnumerable<RepositoryFolder> filteredRepos = repositories
                .Where(repository => repository.Organization.ProjektName.Contains(filter));

            UpdateView(filteredRepos.ToArray());
        }


        /// <summary>
        /// 
        /// Öffnet das zugwiesene Verzeichnis im Explorer.
        /// 
        /// </summary>

        public void OpenProjektFolder()
        {
            TreeNode node = GetNodeBySelection();

            if (null == node || null != node.Parent)
                return;

            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", node.Tag.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Das Projektverzeichnis konnte nicht geöffnet werden! {ex.Message}");
            }
        }


    }
}
