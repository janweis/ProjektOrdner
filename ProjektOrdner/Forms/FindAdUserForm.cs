using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class FindAdUserForm : Form
    {
        public List<AdUser> AdUserResults { get; set; }
        public AppSettings AppSettings { get; set; }

        public AdUser.IdentificationTypes SuchFilter { get; set; }
        public bool SearchForUsers { get; set; }

        public IProgress<string> StatusProgress { get; set; }

        public FindAdUserForm(AppSettings appSettings)
        {
            AdUserResults = new List<AdUser>();
            StatusProgress = new Progress<string>(status => OutputStatus(status));
            AppSettings = appSettings;

            InitializeComponent();
            InitializeControls();
        }


        //
        // Functions
        //

        /// <summary>
        /// 
        /// Initialisiert die Steuerelemente
        /// 
        /// </summary>
        private void InitializeControls()
        {
            FilterArtCombo.SelectedIndex = 0;
            SuchTypCombo.SelectedIndex = 0;
        }


        /// <summary>
        /// 
        /// Sucht nach dem Active Directory Benutzer und fügt ihn in die Liste.
        /// 
        /// </summary>
        private async Task<List<AdUser>> SearchInActiveDirectory(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString) == true)
                return null; // Suche ist leer

            // Search Ad-User
            ActiveDirectoryUtil activeDirectory = new ActiveDirectoryUtil(AppSettings);

            List<AdUser> userList = new List<AdUser>();

            await Task.Run(() =>
            {
                if (SearchForUsers)
                {
                    // Username
                    UserPrincipal foundUser = activeDirectory.GetUserByType(searchString, SuchFilter);

                    if (null != foundUser)
                    {
                        AdUser adUser = new AdUser(foundUser);

                        if (null != adUser)
                            userList.Add(adUser);
                    }
                }
                else
                {
                    // Ad-Group
                    GroupPrincipal foundAdGroup = activeDirectory.GetGroup(searchString, IdentityType.SamAccountName);

                    if (null != foundAdGroup)
                    {
                        foreach (UserPrincipal user in foundAdGroup.Members)
                        {
                            AdUser adUser = new AdUser(user);

                            if (null != adUser)
                                userList.Add(adUser);
                        }
                    }
                }
            });

            return userList;
        }


        /// <summary>
        /// 
        /// Fügt den gefunden Benutzer der Liste hinzu.
        /// 
        /// </summary>
        private void AddResultToView(List<AdUser> adUsers)
        {
            // Add User to View
            PermissionNodeProcessor nodeProcessor = new PermissionNodeProcessor(UserTreeView);
            foreach (AdUser adUser in adUsers)
                nodeProcessor.AddNodeDirect(adUser);
        }


        /// <summary>
        /// 
        /// Setzt den Suchfilter
        /// 
        /// </summary>
        private void SetSeachFilter(ComboBox comboBox)
        {
            switch (comboBox.Text)
            {
                case "Name":
                {
                    SuchFilter = AdUser.IdentificationTypes.SamAccountName;
                    SearchForUsers = true;
                    break;
                }
                case "Matrikelnummer":
                {
                    SuchFilter = AdUser.IdentificationTypes.Matrikelnummer;
                    SearchForUsers = true;
                    break;
                }
                case "E-Mail Adresse":
                {
                    SuchFilter = AdUser.IdentificationTypes.Email;
                    SearchForUsers = true;
                    break;
                }
            }

            StatusProgress.Report("Suchfilter wurde gesetzt!");
        }


        /// <summary>
        /// 
        /// Setzt den Suchtyp
        /// 
        /// </summary>
        private void SetSearchType(ComboBox comboBox)
        {
            switch (comboBox.Text)
            {
                case "Benutzer":
                {
                    SearchForUsers = true;
                    break;
                }
                case "Gruppe":
                {
                    SearchForUsers = false;
                    break;
                }
            }

            StatusProgress.Report("Suchtyp wurde gesetzt!");
        }


        /// <summary>
        /// 
        /// Führt die Suche aus
        /// 
        /// </summary>
        private async Task InvokeSearchAsync()
        {
            List<AdUser> adUsers = null;

            // Set Controls
            AdUserResults.Clear();      // Clear List
            UserTreeView.Nodes.Clear(); // Clear Control
            SuchenButton.Enabled = false;
            AddUserButton.Enabled = false;

            try
            {
                StatusProgress.Report("Suche wird durchgeführt...");
                adUsers = await SearchInActiveDirectory(FilterBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Es ist ein Fehler in der AD-Suche aufgetreten. {ex.Message}", "Suche im AD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                StatusProgress.Report("Suche beendet!");
            }

            // Set to public List for return
            AdUserResults = adUsers;

            // Set Controls
            if (null != adUsers)
                AddResultToView(adUsers);

            SuchenButton.Enabled = true;
            AddUserButton.Enabled = true;
        }


        /// <summary>
        /// 
        /// Setzt den aktuellen Status
        /// 
        /// </summary>
        private void OutputStatus(string message)
        {
            StatusStripLabel.Text = message;
            StatusStripLabel.Invalidate();
        }



        //
        // Controls
        //


        private void AddUserButton_Click(object sender, EventArgs e) =>
            Close();

        private async void SuchenButton_Click(object sender, EventArgs e) =>
            await InvokeSearchAsync();

        private void FilterArtCombo_SelectedIndexChanged(object sender, EventArgs e) =>
            SetSeachFilter(sender as ComboBox);

        private void SuchTypCombo_SelectedIndexChanged(object sender, EventArgs e) =>
            SetSearchType(sender as ComboBox);
    }
}