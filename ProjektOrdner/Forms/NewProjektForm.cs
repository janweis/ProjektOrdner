using System;
using System.IO;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class NewProjektForm : Form
    {
        public string ProjektName { get; set; }
        public DateTime ProjektEnde { get; set; }
        public bool UsePermissionAssistent { get; set; }

        public NewProjektForm()
        {
            ProjektName = string.Empty;
            ProjektEnde = DateTime.Now.AddDays(30);
            UsePermissionAssistent = true;

            InitializeComponent();
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
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void ValidateAndClose()
        {
            string projektName = ProjektnameTextbox.Text;

            bool isValidName = IsValidProjektName(projektName);
            if (isValidName == false)
                return;

            bool isValidDate = IsValidProjektDate(EndDate.Value);
            if (isValidDate == false)
                return;

            ProjektName = projektName;
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
        private bool IsValidProjektName(string projektName)
        {
            // Leer oder Leerzeichen
            if (string.IsNullOrWhiteSpace(projektName) == true)
            {
                MessageBox.Show("Der Projektname darf nicht leer sein!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Länge
            if (projektName.Length > 150)
            {
                MessageBox.Show("Geben Sie einen Namen kleiner 150 Zeichen ein!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Ungültige Zeichen
            if (projektName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                MessageBox.Show("Es wurden ungültige Zeichen im Projektnamen gefunden!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if(UsePermissionAssistent == true)
            {
                UsePermissionAssistent = false;
            }
            else
            {
                UsePermissionAssistent = true;
            }
        }


        //
        // Controls
        //

        private void AbbrechenButton_Click(object sender, EventArgs e) => Close();

        private void AnlegenButton_Click(object sender, EventArgs e) => ValidateAndClose();

        private void SetupPermissionsBox_CheckedChanged(object sender, EventArgs e) => SetStatePermissionAssistent();
    }
}
