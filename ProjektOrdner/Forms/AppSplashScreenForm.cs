using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner.Forms
{
    public partial class AppSplashScreenForm : Form
    {
        public AppSplashScreenForm()
        {
            InitializeComponent();
        }

        private void SplashScreenForm_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }

        public void UpdatePercent(int newValue)
        {
            //LadestatusText.Text = newValue.ToString();
            //LadestatusText.Refresh();
            Refresh();
        }

        public void UpdateProjekt(string projekt)
        {
            LadestatusProjektText.Text = projekt;
            LadestatusProjektText.Refresh();
            Refresh();
        }


    }
}
