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
    public partial class ExpandProjektExpirationForm : Form
    {
        public DateTime NewExpireDate { get; set; }
        private DateTime ProjektExireDate { get; set; }


        public ExpandProjektExpirationForm(string projektName, DateTime projektExpireDate)
        {
            InitializeComponent();
            ProjektExireDate = projektExpireDate;
            NewExpireDate = projektExpireDate;

            NameLabel.Text = projektName;
            AblaufdatumLabel.Text = projektExpireDate.ToShortDateString();
            monthCalendar1.SelectionStart = projektExpireDate;
        }

        private void VerlängernButton_Click(object sender, EventArgs e)
        {
            if(NewExpireDate <= ProjektExireDate)
            {
                DialogResult result = MessageBox.Show("Das gewählte Datum verkürzt die Projektlaufzeit. Wollen Sie das wirklich?","Frage",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e) => NewExpireDate = e.Start;

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e) => NewExpireDate = e.Start;
    }
}
