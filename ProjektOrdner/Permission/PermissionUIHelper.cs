using ProjektOrdner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Permission
{
    public static class PermissionUIHelper
    {

        public static void AddUserToTreeView(TreeView treeView, AdUserModel adUser)
        {
            string name = $"{adUser.Nachname}, {adUser.Vorname}";

            treeView.BeginUpdate();

            treeView.Nodes.Add(name, name, 0, 0);
            treeView.Nodes[name].Nodes.Add("", $"Benutzername:{adUser.SamAccountName}", 1, 1);
            treeView.Nodes[name].Nodes.Add("", $"Email:{adUser.Email}", 1, 1);
            treeView.Nodes[name].Nodes.Add("", $"Matrikelnummer:{adUser.Matrikelnummer}", 1, 1);
            treeView.Nodes[name].Nodes.Add("", $"UPN:{adUser.UserPrincipalName}", 1, 1);

            treeView.EndUpdate();
        }

        public static TreeNode CreatePermissionNode(PermissionModel projektPermission)
        {
            string name = $"{projektPermission.User.Nachname}, {projektPermission.User.Vorname}";
            TreeNode[] childNodes =
            {
                new TreeNode($"Benutzername:{projektPermission.User.SamAccountName}", 1, 1),
                new TreeNode($"Email:{projektPermission.User.EMail}", 1, 1),
                new TreeNode( $"Matrikelnummer:{projektPermission.User.Matrikelnummer}", 1, 1),
                new TreeNode($"UPN: - ", 1, 1)
            };

            return new TreeNode(name, 0, 0, childNodes);
        }

        public static TreeNode CreateNode(AdUserModel adUser)
        {
            string name = $"{adUser.Nachname}, {adUser.Vorname}";
            TreeNode[] childNodes =
            {
                new TreeNode($"Benutzername:{adUser.SamAccountName}", 1, 1),
                new TreeNode($"Email:{adUser.Email}", 1, 1),
                new TreeNode($"Matrikelnummer:{adUser.Matrikelnummer}", 1, 1),
                new TreeNode($"UPN:{adUser.UserPrincipalName}", 1, 1)
            };

            return new TreeNode(name, 0, 0, childNodes);
        }

    }
}
