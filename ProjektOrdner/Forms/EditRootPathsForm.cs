using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class EditRootPathsForm : Form
    {
        public List<string> RootPaths { get; set; }
        private List<string> InitalRoots { get; set; }

        public EditRootPathsForm(List<string> existingPaths = null)
        {
            InitializeComponent();

            InitalRoots = existingPaths;
            if (null != existingPaths)
            {
                RootPathsTextBox.Lines = existingPaths.ToArray();
            }
        }


        //
        // FUNCTIONS
        // ___________________________________________________________________________________


        /// <summary>
        /// 
        /// Fügt einen neuen Pfad zu der Textbox hinzu.
        /// 
        /// </summary>

        private void AddRootPath()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Wählen Sie einen oder mehrere Root-Ordner aus.";
            DialogResult dialogResult = folderBrowser.ShowDialog();

            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                if (dialogResult == DialogResult.OK)
                {
                    if (string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) == false)
                    {
                        RootPathsTextBox.Text += folderBrowser.SelectedPath + "; ";
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// Übergibt die Pfade in die Variable und beendet die Form.
        /// 
        /// </summary>

        private void ExitDialog()
        {
            // Define valid paths
            IEnumerable<string> paths = RootPathsTextBox.Text.Split(';').Where(path => string.IsNullOrWhiteSpace(path) == false);

            if (null != paths)
            {
                RootPaths = paths.ToList();
            }

            // Exit
            Close();
        }



        //
        // CONTROLS
        // ___________________________________________________________________________________

        private void DurchsuchenButton_Click(object sender, EventArgs e)
        {
            AddRootPath();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ExitDialog();
        }

        private void AbbrechenButton_Click(object sender, EventArgs e)
        {
            ExitDialog();
        }

    }
}
