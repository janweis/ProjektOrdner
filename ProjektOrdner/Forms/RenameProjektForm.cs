using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class RenameProjektForm : Form
    {
        public string ProjektName { get; set; }
        private string CurrentProjektName { get; set; }

        public RenameProjektForm(string currentProjektName)
        {
            CurrentProjektName = currentProjektName;
            ProjektName = string.Empty;

            InitializeComponent();
            PreSetComponents();
        }

        private void PreSetComponents()
        {
            ProjektnameLbl.Text = CurrentProjektName;
        }

        private void RenameProjektButton_Click(object sender, EventArgs e)
        {
            bool isProjektNameValid = IsNameValid(ProjektNameTextBox.Text);
            if (isProjektNameValid == false)
                return;

            ProjektName = ProjektNameTextBox.Text;

            DialogResult = DialogResult.OK;
            Close();
        }


        private void AbbrechenButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


        //
        // Validations
        // 

        private bool IsNameValid(string projektName)
        {
            // Leer oder Leerzeichen
            if (string.IsNullOrWhiteSpace(projektName) == true)
            {
                MessageBox.Show("Der Projektname darf nicht leer sein!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Länge
            if (ProjektName.Length > 150)
            {
                MessageBox.Show("Geben Sie einen Namen kleiner 150 Zeichen ein!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Ungültige Zeichen
            if (ProjektName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                MessageBox.Show("Der neue Name enthält ungültige Zeichen!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}
