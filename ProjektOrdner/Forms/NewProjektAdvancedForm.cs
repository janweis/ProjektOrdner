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
    public partial class TextboxForm : Form
    {
        public string[] TextBoxContent { get; set; }

        public TextboxForm(string[] textInhalt)
        {
            InitializeComponent();
            TextBoxContent = textInhalt;
            SetupTextboxText();
        }

        private void SetupTextboxText()
        {
            TextBox.Lines = TextBoxContent;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            TextBoxContent = TextBox.Lines;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
