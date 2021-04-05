using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.App
{
    public class TreeViewHelper
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        private TreeView View { get; set; }
        private ContextMenuStrip Context { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public TreeViewHelper(TreeView treeView, ContextMenuStrip contextMenuStrip)
        {
            View = treeView;
            Context = contextMenuStrip;
        }

        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 

        //
        // Get Node
        //

        /// <summary>
        /// 
        /// </summary>
        public TreeNode GetNodeByName(string Name)
        {
            if (View.Nodes.ContainsKey(Name) == true)
            {
                return View.Nodes[Name];
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public TreeNode GetNodeByTag(string tag)
        {
            foreach (TreeNode node in View.Nodes)
            {
                if (node.Tag.ToString() == tag)
                {
                    return node;
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public TreeNode GetNodeBySelection(bool onlyMasterNode = false)
        {
            if (onlyMasterNode == false)
                return View.SelectedNode;

            if (null == View.SelectedNode)
            {
                return null;
            }
            else
            {
                if (null == View.SelectedNode.Parent)
                {
                    return View.SelectedNode;
                }
                else
                {
                    return View.SelectedNode.Parent;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<TreeNode> GetNodesByChecked()
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


        /// <summary>
        /// 
        /// Ruft ein Repository anhand des Names in einem Node ab
        /// 
        /// </summary>
        public RepositoryFolder GetRepositoryFromNode(RepositoryFolder[] repositories)
        {
            var selectedNode = GetNodeBySelection();

            if (null == selectedNode)
                return null;

            if (null == repositories)
                return null;

            return repositories
                .Where(repo => selectedNode.Name == repo.Organization.ProjektName)
                .FirstOrDefault();
        }


        //
        // Add Node
        //

        /// <summary>
        /// 
        /// </summary>
        public void AddProjektNodeCorrupted(string Name, string projektPath)
        {
            // Add Projekt Node
            View.Nodes.Add(Name, Name, 5, 5);
            View.Nodes[Name].Tag = projektPath;
            View.Nodes[Name].NodeFont = new System.Drawing.Font(View.Font, System.Drawing.FontStyle.Italic);
        }


        /// <summary>
        /// 
        /// </summary>
        public void AddProjektNodeV2(string Name, string projektPath)
        {
            // Add Projekt Node
            View.Nodes.Add(Name, Name, 6, 6);
            View.Nodes[Name].Tag = projektPath;

            // Add Attribute Nodes
            View.Nodes[Name].ContextMenuStrip = Context;
            View.Nodes[Name].Nodes.Add("Manager", "Manager", 1, 1);
            View.Nodes[Name].Nodes.Add("Member", "Mitarbeiter", 1, 1);
            View.Nodes[Name].Nodes.Add("Guest", "Gäste", 1, 1);
            View.Nodes[Name].Nodes.Add("Settings", "Einstellungen", 0, 0);
            View.Nodes[Name].Nodes.Add("Organization", "ProjektOrdner", 2, 2);
        }


        /// <summary>
        /// 
        /// </summary>
        public void AddProjektNodeV1(string Name, string projektPath)
        {
            // Add Projekt Node
            View.Nodes.Add(Name, Name, 6, 6);
            View.Nodes[Name].Tag = projektPath;

            // Add Attribute Nodes
            View.Nodes[Name].ContextMenuStrip = Context;
            View.Nodes[Name].Nodes.Add("Member", "Lesen & Schreiben", 1, 1);
            View.Nodes[Name].Nodes.Add("Guest", "Nur Lesen", 1, 1);
            View.Nodes[Name].Nodes.Add("Organization", "ProjektOrdner v1", 2, 2);
        }


        //
        // Remove Node
        // 


        /// <summary>
        /// 
        /// </summary>
        public void RemoveProjektNodeByName(string Name)
        {
            TreeNode node = GetNodeByName(Name);

            if (null != node)
                View.Nodes.Remove(node);
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveProjektNodeByID(string id)
        {
            TreeNode node = GetNodeByTag(id);

            if (null != node)
                View.Nodes.Remove(node);
        }


        //
        // CLEAR
        //

        /// <summary>
        /// 
        /// Clear all Nodes
        /// 
        /// </summary>
        public void ClearView()
        {
            View.BeginUpdate();
            View.Nodes.Clear();
            View.EndUpdate();
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
            // Clear
            if (null == repositories || repositories.Count() == 0)
            {
                View.BeginUpdate();
                View.Nodes.Clear();
                View.EndUpdate();

                return;
            }

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
        public async Task SetFilterViewAsync(string filter, RepositoryFolder[] repositories, IProgress<int> repoCounter, IProgress<string> progressMessage, AppSettings appSettings)
        {
            IEnumerable<RepositoryFolder> filteredRepos = null;
            if (filter.Contains("filter=", StringComparison.OrdinalIgnoreCase) == false)
            {
                filteredRepos = repositories
                    .Where(repository => repository.Organization.ProjektName.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                string filterRequest = filter
                    .Substring(filter.IndexOf("filter=") + 7)
                    .ToLower();

                if(filterRequest.Contains("expired"))
                {
                    filteredRepos = repositories
                        .Where(repository => repository.Organization.ProjektEnde < DateTime.Today);
                }
                else if(filterRequest.Contains("person="))
                {
                    string personFilter = filterRequest.Substring(filterRequest.IndexOf("person=") + 7);
                    if (personFilter == "" || personFilter == "exampleuser")
                        return;

                    progressMessage.Report($"Suche Benutzer {personFilter} in Projekten...");
                    List<RepositoryFolder> foundRepositories = new List<RepositoryFolder>();
                    foreach (RepositoryFolder repository in repositories)
                    {
                        PermissionProcessor permissionProcessor = new PermissionProcessor(repository, appSettings);
                        RepositoryPermission[] repositoryPermissions = await permissionProcessor.GetPermissionsAsync();

                        RepositoryPermission foundRequiredUser = repositoryPermissions
                            .ToList()
                            .Find(permission => permission.User.SamAccountName == personFilter);

                        if (null != foundRequiredUser)
                            foundRepositories.Add(repository);
                    }

                    filteredRepos = foundRepositories;
                    progressMessage.Report($"Benutzersuche abgeschlossen!");
                }
            }

            if (null == filteredRepos)
                return;

            repoCounter.Report(filteredRepos.Count());
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
