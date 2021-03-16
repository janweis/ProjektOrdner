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
        public List<AdUser> AdUsers { get; set; }
        public AdUser CurrentUserShown { get; set; }
        public AppSettings AppSettings { get; set; }


        public FindAdUserForm(List<AdUser> users, AppSettings appSettings)
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

            AdUser.IdentificationTypes identification = AdUser.IdentificationTypes.SamAccountName;
            switch (FilterArtCombo.Text)
            {
                case "Benutzername":
                {
                    identification = AdUser.IdentificationTypes.SamAccountName;
                    break;
                }
                case "Matrikelnummer":
                {
                    identification = AdUser.IdentificationTypes.Matrikelnummer;
                    break;
                }
                case "E-Mail Adresse":
                {
                    identification = AdUser.IdentificationTypes.Email;
                    break;
                }
            }

            // Search Ad-User
            ActiveDirectoryUtil activeDirectory = new ActiveDirectoryUtil(AppSettings);
            UserPrincipal foundUser = activeDirectory.GetUserByType(FilterBox.Text, identification);

            if (null == foundUser)
                return; // Kein Benutzer gefunden!

            AdUser user = new AdUser(foundUser);

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
