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
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        private List<string> InitalRoots { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public EditRootPathsForm(List<string> existingPaths)
        {
            InitializeComponent();

            if (null == existingPaths)
            {
                InitalRoots = new List<string>();
            }
            else
            {
                InitalRoots = existingPaths;
            }

            if (InitalRoots.Count > 0)
            {
                RootPathsTextBox.Lines = existingPaths.ToArray();
            }
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Functions
        // 

        /// <summary>
        /// 
        /// Fügt einen neuen Pfad zu der Textbox hinzu.
        /// 
        /// </summary>
        private void OpenFolderDialog()
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
        /// Set root paths
        /// 
        /// </summary>
        private void ApplyRootFolders()
        {
            InitalRoots.Clear();
            InitalRoots.AddRange(RootPathsTextBox.Text.Split(';').Where(root => string.IsNullOrWhiteSpace(root) == false));
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Controls
        // 

        private void DurchsuchenButton_Click(object sender, EventArgs e) => OpenFolderDialog();

        private void OkButton_Click(object sender, EventArgs e)
        {
            ApplyRootFolders();
            Close();
        }

        private void AbbrechenButton_Click(object sender, EventArgs e) => Close();
    }
}
