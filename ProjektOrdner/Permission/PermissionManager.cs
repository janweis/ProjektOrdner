using ProjektOrdner.App;
using ProjektOrdner.Forms;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Permission
{
    public class PermissionManager
    {
        private string FolderPath { get; set; }
        private AppSettings AppSettings { get; set; }

        public PermissionManager(string folderPath, AppSettings appSettings)
        {
            FolderPath = folderPath;
            AppSettings = appSettings;
        }

        //
        // Functions
        //

        public async Task ManagePermissions()
        {
            // Get current permissions
            PermissionProcessor permissionProcessor = new PermissionProcessor(FolderPath, AppSettings);
            RepositoryPermission[] projektPermissions = await permissionProcessor.GetPermissionsAsync();

            // Prepare second Permission List
            List<RepositoryPermission> editedPermissions = new List<RepositoryPermission>();
            if (null != projektPermissions)
            {
                editedPermissions.AddRange(projektPermissions);
            }

            ManagePermissionsForm permissionManage = new ManagePermissionsForm(editedPermissions, AppSettings);
            DialogResult dialogResult = permissionManage.ShowDialog();

            if (dialogResult == DialogResult.Cancel)
                return;

            // Update Permission
            await permissionProcessor.SyncPermissionsAsync(editedPermissions.ToArray());
        }

    }
}
