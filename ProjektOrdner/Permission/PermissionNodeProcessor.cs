using ProjektOrdner.App;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace ProjektOrdner.Permission
{
    public class PermissionNodeProcessor
    {
        private TreeView View { get; set; }

        public enum MasterNode
        {
            [Description("Nicht berechtigt")]
            Unbestimmt,
            [Description("Manager")]
            Manager,
            [Description("Mitarbeiter")]
            Mitarbeiter,
            [Description("Nur Lesen")]
            NurLesen
        }

        public PermissionNodeProcessor(TreeView treeView)
        {
            View = treeView;
        }


        /// <summary>
        /// 
        /// Initalisiert den TreeView mit den Standardknoten
        /// 
        /// </summary>
        public void ResetView()
        {
            // Prepare
            View.Nodes.Clear();
            View.BeginUpdate();

            // Add Master Nodes
            AddMasterNode(MasterNode.Unbestimmt, 1);
            AddMasterNode(MasterNode.Manager, 2);
            AddMasterNode(MasterNode.Mitarbeiter, 2);
            AddMasterNode(MasterNode.NurLesen, 2);

            // Finish
            View.EndUpdate();
        }


        /// <summary>
        /// 
        /// </summary>
        public void CollapseAll()
        {
            View.CollapseAll();
        }


        /// <summary>
        /// 
        /// </summary>
        public void ExpandAll()
        {
            View.ExpandAll();
        }


        /// <summary>
        /// 
        /// Erstellt die Ansicht im TreeView
        /// 
        /// </summary>

        public void UpdateView(RepositoryPermission[] permissions, bool advancedView)
        {
            ResetView();
            View.BeginUpdate();

            foreach (RepositoryPermission permission in permissions)
            {
                // Process every permission

                switch (permission.Role)
                {
                    case PermissionRole.Undefined:
                    {
                        if (advancedView == true)
                        {
                            AddNodeAdvanced(permission.User, MasterNode.Unbestimmt);
                        }
                        else
                        {
                            AddNodeSimple(permission.User, MasterNode.Unbestimmt);
                        }

                        break;
                    }
                    case PermissionRole.Guest:
                    {
                        if (advancedView == true)
                        {
                            AddNodeAdvanced(permission.User, MasterNode.NurLesen);
                        }
                        else
                        {
                            AddNodeSimple(permission.User, MasterNode.NurLesen);
                        }

                        break;
                    }
                    case PermissionRole.Member:
                    {
                        if (advancedView == true)
                        {
                            AddNodeAdvanced(permission.User, MasterNode.Mitarbeiter);
                        }
                        else
                        {
                            AddNodeSimple(permission.User, MasterNode.Mitarbeiter);
                        }

                        break;
                    }
                    case PermissionRole.Manager:
                    {
                        if (advancedView == true)
                        {
                            AddNodeAdvanced(permission.User, MasterNode.Manager);
                        }
                        else
                        {
                            AddNodeSimple(permission.User, MasterNode.Manager);
                        }

                        break;
                    }
                }
            }

            View.EndUpdate();
        }


        /// <summary>
        /// 
        /// Entfernt alle Einträge aus einem Masternode.
        /// 
        /// </summary>

        public void ClearMasterNode(MasterNode masterNode)
        {
            TreeNode node = GetNodeByName(masterNode.ToString());

            if (null != node)
                node.Nodes.Clear();
        }


        //
        // Get Node
        //

        public TreeNode GetNodeByName(string name)
        {
            if (View.Nodes.ContainsKey(name) == true)
            {
                return View.Nodes[name];
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

        public TreeNode GetNodeBySelection()
        {
            return View.SelectedNode;
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
                    if (node.Checked == true)
                        nodeList.Add(node);
                }
            }

            return nodeList;
        }

        public List<TreeNode> GetNodesByCheckBoxes(bool includeSubnodes)
        {
            List<TreeNode> nodeList = null;

            if (View.CheckBoxes == true)
            {
                nodeList = new List<TreeNode>();

                // Get checked Projects
                foreach (TreeNode node in View.Nodes)
                {
                    if (includeSubnodes)
                    {
                        if (null != node.Nodes)
                        {
                            // SubNodes
                            foreach (TreeNode subNode in node.Nodes)
                            {
                                if (subNode.Checked == true)
                                    nodeList.Add(subNode);
                            }
                        }
                    }
                    else
                    {
                        if (node.Checked == true)
                            nodeList.Add(node);
                    }
                }
            }

            return nodeList;
        }

        public TreeNode GetMasterNode(MasterNode masterNode)
        {
            string masterNodeName = GetDescription(masterNode);
            return GetNodeByName(masterNodeName);
        }


        //
        // Add Node
        //

        public void AddMasterNode(MasterNode masterNode, int imageID)
        {
            string masterNodeName = GetDescription(masterNode);

            // Add Master Node
            View.Nodes.Add(masterNodeName, masterNodeName, imageID, imageID);
            View.Nodes[masterNodeName].Tag = masterNodeName;
        }

        public void AddNodeAdvanced(AdUser adUser, MasterNode master)
        {
            if (null == adUser)
                return;

            // Get master node
            TreeNode masterNode = GetMasterNode(master);

            // Add Node
            string name = $"{adUser.Nachname}, {adUser.Vorname}";
            masterNode.Nodes.Add(name, name, 0, 0);
            masterNode.Nodes[name].Tag = name;

            // Add Node Attributes
            masterNode.Nodes[name].Nodes.Add("", $"Benutzername:{adUser.SamAccountName}", 1, 1);
            masterNode.Nodes[name].Nodes.Add("", $"Email:{adUser.Email}", 1, 1);
            masterNode.Nodes[name].Nodes.Add("", $"Matrikelnummer:{adUser.Matrikelnummer}", 1, 1);
            masterNode.Expand();

            SetFocusToNode(View.Nodes[name]);
        }

        public void AddNodeSimple(AdUser adUser, MasterNode master)
        {
            if (null == adUser)
                return;

            // Get master node
            TreeNode masterNode = GetMasterNode(master);

            // Add Projekt Node
            string name = $"{adUser.Nachname}, {adUser.Vorname}";
            masterNode.Nodes.Add(name, name, 0, 0);
            masterNode.Nodes[name].Tag = name;
            masterNode.Expand();

            SetFocusToNode(View.Nodes[name]);
        }

        public void AddNodeDirect(AdUser adUser)
        {
            // Add Node
            string name = $"{adUser.Nachname}, {adUser.Vorname}";
            View.Nodes.Add(name, name, 0, 0);
            View.Nodes[name].Tag = name;

            // Add Node Attributes
            View.Nodes[name].Nodes.Add("", $"Benutzername: {adUser.SamAccountName}", 1, 1);
            View.Nodes[name].Nodes.Add("", $"Email: {adUser.Email}", 1, 1);
            View.Nodes[name].Nodes.Add("", $"Matrikelnummer: {adUser.Matrikelnummer}", 1, 1);
            //View.Nodes[name].Expand();

            SetFocusToNode(View.Nodes[name]);
        }

        private void SetFocusToNode(TreeNode node)
        {
            View.SelectedNode = node;
            View.Focus();
        }


        //
        // Move Node
        //

        public void MoveNodeTo(TreeNode node, MasterNode master)
        {
            if (null == node)
                return; // Kein Node vorhanden

            TreeNode masterNode = GetMasterNode(master);
            if (null == masterNode)
                return; // Kein Masternode vorhanden

            node.Parent.Nodes.Remove(node);  // Remove Node
            masterNode.Nodes.Add(node);     // Add Node
            masterNode.Expand();           // Expand View

            SetFocusToNode(node);
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
        // Helpers
        //

        public static string GetDescription<T>(T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

    }
}
