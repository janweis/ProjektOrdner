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
        private string CurrentName { get; set; }

        public RenameProjektForm(string currentName)
        {
            CurrentName = currentName;
            Name = string.Empty;

            InitializeComponent();
            PreSetComponents();
        }

        private void PreSetComponents()
        {
            NameLbl.Text = CurrentName;
        }

        private void RenameProjektButton_Click(object sender, EventArgs e)
        {
            bool isNameValid = IsNameValid(NameTextBox.Text);
            if (isNameValid == false)
                return;

            Name = NameTextBox.Text;

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

        private bool IsNameValid(string Name)
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
                MessageBox.Show("Der neue Name enthält ungültige Zeichen!", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}
