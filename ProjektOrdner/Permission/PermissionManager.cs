using ProjektOrdner.App;
using ProjektOrdner.Forms;
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
        private AppSettingsModel AppSettings { get; set; }

        public PermissionManager(string folderPath, AppSettingsModel appSettings)
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
            PermissionModel[] projektPermissions = null;
            try
            {
                projektPermissions = await permissionProcessor.GetPermissionsAsync(PermissionSource.File);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler beim Lesen der Projektberechtigungen aufgetreten!\n{ex.Message}", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Manage permissions
            List<PermissionModel> editedPermissions = new List<PermissionModel>();
            if (null != projektPermissions)
            {
                editedPermissions.AddRange(projektPermissions);
            }


            try
            {
                ManagePermissionsForm permissionManage = new ManagePermissionsForm(editedPermissions, AppSettings);
                DialogResult dialogResult = permissionManage.ShowDialog();

                if (dialogResult == DialogResult.Cancel)
                    return;

                // Update Permission
                await permissionProcessor.UpdatePermissionsAsync(editedPermissions.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten!\n{ex.Message}");
                return;
            }
        }

    }
}
