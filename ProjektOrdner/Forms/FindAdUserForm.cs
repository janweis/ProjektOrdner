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
        public bool SeachForUsers { get; set; }

        public FindAdUserForm(AppSettings appSettings)
        {
            AdUserResults = new List<AdUser>();
            AppSettings = appSettings;

            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            FilterArtCombo.SelectedIndex = 0;
        }

        // Functions

        /// <summary>
        /// 
        /// Sucht nach dem Active Directory Benutzer und fügt ihn in die Liste.
        /// 
        /// </summary>
        private async Task<List<AdUser>> SearchForAdObjectAsync(string searchString)
        {
            // Clear View
            UserTreeView.Nodes.Clear();

            if (string.IsNullOrWhiteSpace(searchString) == true)
                return null; // Suche ist leer

            // Search Ad-User
            ActiveDirectoryUtil activeDirectory = new ActiveDirectoryUtil(AppSettings);

            List<AdUser> userList = new List<AdUser>();

            await Task.Run(() =>
            {
                if (SeachForUsers)
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



        // Controls

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void SuchenButton_Click(object sender, EventArgs e)
        {
            AdUserResults.Clear();
            List<AdUser> AdUsers = await SearchForAdObjectAsync(FilterBox.Text);

            if (null != AdUsers)
                AddResultToView(AdUsers);

            AdUserResults = AdUsers;
        }

        private void FilterArtCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            switch (comboBox.Text)
            {
                case "Benutzername":
                {
                    SuchFilter = AdUser.IdentificationTypes.SamAccountName;
                    SeachForUsers = true;
                    break;
                }
                case "Matrikelnummer":
                {
                    SuchFilter = AdUser.IdentificationTypes.Matrikelnummer;
                    SeachForUsers = true;
                    break;
                }
                case "E-Mail Adresse":
                {
                    SuchFilter = AdUser.IdentificationTypes.Email;
                    SeachForUsers = true;
                    break;
                }
                case "Domänen-Gruppe":
                {
                    SuchFilter = AdUser.IdentificationTypes.SamAccountName;
                    SeachForUsers = false;
                    break;
                }
            }
        }
    }
}