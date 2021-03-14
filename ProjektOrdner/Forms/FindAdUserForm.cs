using ProjektOrdner.App;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class FindAdUserForm : Form
    {
        public List<UserModel> AdUsers { get; set; }
        public UserModel CurrentUserShown { get; set; }
        public AppSettingsModel AppSettings { get; set; }


        public FindAdUserForm(List<UserModel> users, AppSettingsModel appSettings)
        {
            AdUsers = users;
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

        private void SeachForAdUser()
        {
            // Clear View
            UserTreeView.Nodes.Clear();

            if (string.IsNullOrWhiteSpace(FilterBox.Text) == true)
                return; // Suche ist leer

            UserModel.IdentificationTypes identification = UserModel.IdentificationTypes.SamAccountName;
            switch (FilterArtCombo.Text)
            {
                case "Benutzername":
                {
                    identification = UserModel.IdentificationTypes.SamAccountName;
                    break;
                }
                case "Matrikelnummer":
                {
                    identification = UserModel.IdentificationTypes.Matrikelnummer;
                    break;
                }
                case "E-Mail Adresse":
                {
                    identification = UserModel.IdentificationTypes.Email;
                    break;
                }
            }

            // Search Ad-User
            ActiveDirectoryUtil activeDirectory = new ActiveDirectoryUtil(AppSettings);
            UserPrincipal foundUser = activeDirectory.GetUserByType(FilterBox.Text, identification);

            if (null == foundUser)
                return; // Kein Benutzer gefunden!

            UserModel user = new UserModel(foundUser);

            // Add User to View
            PermissionNodeProcessor nodeProcessor = new PermissionNodeProcessor(UserTreeView);
            nodeProcessor.AddNodeDirect(user);

            CurrentUserShown = user;
        }


        /// <summary>
        /// 
        /// Fügt den gefunden Benutzer der Liste hinzu.
        /// 
        /// </summary>

        private void AddCurrentUserToList()
        {
            AdUsers.Add(CurrentUserShown);
        }


        // Controls

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            AddCurrentUserToList();
            Close();
        }

        private void SuchenButton_Click(object sender, EventArgs e) => SeachForAdUser();
    }
}
