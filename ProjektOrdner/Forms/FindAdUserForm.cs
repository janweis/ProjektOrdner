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
        public bool IsConfiguredForTeams { get; set; }

        public FindAdUserForm(AppSettings appSettings)
        {
            AdUserResults = new List<AdUser>();
            StatusProgress = new Progress<string>(status => OutputStatus(status));
            AppSettings = appSettings;
            IsConfiguredForTeams = false;

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

        private async Task AddSettingTeamsAsync()
        {
            AppSettings.Teams.AddRange(new SettingTeam[]
            {
                new SettingTeam("Adrive","GG-R-Adrive-Mitarbeiter"),
                new SettingTeam("Adrive","GG-R-Adrive-Leitung"),
                new SettingTeam("PowerElectronics","GG-R-PowerElectronics-Mitarbeiter"),
                new SettingTeam("PowerElectronics","GG-R-PowerElectronics-Leitung")
            });

            await AppSettings.SaveAsync();
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
                    SetControlsToDefault();
                    break;
                }
                case "Gruppe":
                {
                    SearchForUsers = false;
                    SetControlsToDefault();
                    break;
                }
                case "Teams (mehrere Gruppen)":
                {
                    SearchForUsers = false;
                    SetControlsForTeams();
                    break;
                }
            }

            StatusProgress.Report("Suchtyp wurde gesetzt!");
        }


        /// <summary>
        /// 
        /// Set Controls for Teams
        /// 
        /// </summary>
        private void SetControlsForTeams()
        {
            if (IsConfiguredForTeams == false)
            {
                // Set Controls
                FilterArtCombo.Items.Clear();
                FilterBox.Enabled = false;
                SearchForUsers = false;

                // Add Teams
                if (null != AppSettings.Teams)
                {
                    foreach (SettingTeam settingTeam in AppSettings.Teams)
                    {
                        if (FilterArtCombo.Items.Contains(settingTeam.TeamName) == false)
                            FilterArtCombo.Items.Add(settingTeam.TeamName);
                    }
                }

                // Set Focus
                if (FilterArtCombo.Items.Count > 0)
                    FilterArtCombo.SelectedIndex = 0;

                IsConfiguredForTeams = true;
            }
        }


        /// <summary>
        /// 
        /// Set Controls to default
        /// 
        /// </summary>
        private void SetControlsToDefault()
        {
            if (IsConfiguredForTeams == true)
            {
                FilterArtCombo.Items.Clear();
                FilterArtCombo.Items.AddRange(new string[] { "Name", "E-Mail Adresse", "Matrikelnummer" });
                FilterArtCombo.SelectedIndex = 0;
                FilterBox.Enabled = true;

                IsConfiguredForTeams = false;
            }
        }


        /// <summary>
        /// 
        /// Führt die Suche aus
        /// 
        /// </summary>
        private async Task InvokeSearchAsync()
        {
            List<AdUser> adUsers = new List<AdUser>();

            // Set Controls
            AdUserResults.Clear();      // Clear List
            UserTreeView.Nodes.Clear(); // Clear Control
            SuchenButton.Enabled = false;
            AddUserButton.Enabled = false;

            try
            {
                StatusProgress.Report("Suche wird durchgeführt...");

                if (IsConfiguredForTeams == false)
                {
                    adUsers = await SearchInActiveDirectory(FilterBox.Text);
                }
                else
                {
                    // Teams suche
                    int i = 1;
                    List<SettingTeam> teams = AppSettings?.Teams?.FindAll(settingTeam => settingTeam.TeamName == FilterArtCombo.Text);
                    int teamsCount = teams.Count;

                    foreach (SettingTeam team in teams)
                    {
                        StatusProgress.Report($"Suche wird durchgeführt... ({i.ToString()}/{teamsCount.ToString()})");

                        List<AdUser> tempUsers = await SearchInActiveDirectory(team.AdGroupSamAccountName);
                        if (null != tempUsers)
                            adUsers.AddRange(tempUsers.ToArray());

                        i++;
                    }
                }
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

        private async void button1_Click(object sender, EventArgs e)
        {
            await AddSettingTeamsAsync();
            StatusProgress.Report("Gespeichert!");
        }
    }
}