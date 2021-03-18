using System;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class RootFolderAssistentForm : Form
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        public int RadioResult { get; set; }
        public string SelectedPath { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public RootFolderAssistentForm()
        {
            InitializeComponent();
            RadioResult = -1;
            SelectedPath = "";
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Functions
        // 

        private void EnableOK()
        {
            OkButton.Enabled = true;
        }

        private void SetResult(int resultId, RadioButton radio)
        {
            if (radio.Checked == true)
            {
                RadioResult = resultId;
                EnableOK();
            }
        }

        private void SelectPath()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult dialogResult = folderBrowser.ShowDialog();

            if (dialogResult == DialogResult.Cancel)
                return;

            PfadTextBox.Text = folderBrowser.SelectedPath;
        }

        // // // // // // // // // // // // // // // // // // // // //
        // Controls
        // 

        private void CancelButton_Click(object sender, EventArgs e) => 
            Close();

        private void OkButton_Click(object sender, EventArgs e)
        {
            SelectedPath = PfadTextBox.Text;
            Close();
        }

        private void EditRootRadio_CheckedChanged(object sender, EventArgs e) => 
            SetResult(0, sender as RadioButton);

        private void CreateRootRadio_CheckedChanged(object sender, EventArgs e) => 
            SetResult(1, sender as RadioButton);

        private void DurchsuchenButton_Click(object sender, EventArgs e) => 
            SelectPath();
    }
}
