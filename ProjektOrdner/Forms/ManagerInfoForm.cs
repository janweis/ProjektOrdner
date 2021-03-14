using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class ManagerInfoForm : Form
    {
        private Version ProgramVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public ManagerInfoForm()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            VersionLabel.Text = ProgramVersion.ToString();
        }
    }
}
