using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uno
{
    public partial class Form_PauseMenu : Form
    {
        public Form_PauseMenu()
        {
            InitializeComponent();
            this.FormClosing += gameForm_FormClosing;
        }

        private void closeBT_Click(object sender, EventArgs e)
        {
            // Find the main form.
            Form mainForm = Application.OpenForms[0];

            // Close the main form.
            mainForm.Close();
        }

        private void gameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
