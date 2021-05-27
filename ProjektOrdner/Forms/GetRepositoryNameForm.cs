using System;
using System.IO;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class GetRepositoryNameForm : Form
    {
        public string ProjektName { get; set; }
        public DateTime ProjektEnde { get; set; }
        public bool UsePermissionAssistent { get; set; }
        private bool IsInfiniteDate { get; set; }

        public GetRepositoryNameForm(string projektName = "")
        {
            InitializeComponent();

            ProjektName = projektName;
            ProjektEnde = DateTime.Now.AddDays(30);
            UsePermissionAssistent = true;
            IsInfiniteDate = false;

            PreSetComponents();
        }

        //
        // Functions
        //

        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void PreSetComponents()
        {
            EndDate.Value = EndDate.Value.AddDays(30);
            NameTextbox.Text = ProjektName;
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void ValidateAndClose()
        {
            bool isValidName = IsValidName(NameTextbox.Text);
            if (isValidName == false)
                return;

            bool isValidDate = IsValidProjektDate(EndDate.Value);
            if (isValidDate == false)
                return;

            ProjektName = NameTextbox.Text;

            if (IsInfiniteDate)
                ProjektEnde = DateTime.MaxValue;
            else
                ProjektEnde = EndDate.Value;

            // Close Assistent
            DialogResult = DialogResult.OK;
            Close();
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private bool IsValidProjektDate(DateTime value)
        {
            if (value.Year > DateTime.Now.AddYears(5).Year)
            {
                MessageBox.Show("Projekte können nicht länger als 5 Jahre angelegt werden!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private bool IsValidName(string Name)
        {
            // Leer oder Leerzeichen
            if (string.IsNullOrWhiteSpace(Name) == true)
            {
                MessageBox.Show("Der Name darf nicht leer sein!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Länge
            if (Name.Length > 150)
            {
                MessageBox.Show("Geben Sie einen Namen kleiner 150 Zeichen ein!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Ungültige Zeichen
            if (Name.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                MessageBox.Show("Es wurden ungültige Zeichen im Namen gefunden!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void SetStatePermissionAssistent()
        {
            if (UsePermissionAssistent == true)
            {
                UsePermissionAssistent = false;
            }
            else
            {
                UsePermissionAssistent = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InfinityDate(bool set)
        {
            if (set)
            {
                EndDate.Enabled = false;
                IsInfiniteDate = true;
            }
            else
            {
                EndDate.Enabled = true;
                IsInfiniteDate = false;
            }
        }


        //
        // Controls
        //

        private void AbbrechenButton_Click(object sender, EventArgs e) => Close();

        private void AnlegenButton_Click(object sender, EventArgs e) => ValidateAndClose();

        private void SetupPermissionsBox_CheckedChanged(object sender, EventArgs e) => SetStatePermissionAssistent();

        private void DateInfinityCheck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            InfinityDate(check.Checked);
        }

    }
}
