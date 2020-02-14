using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFSTTelemetry
{
    public delegate void ButtonClickedCallback(object sender, EventArgs e);


    public partial class ScanDialog : Form
    {
        public ButtonClickedCallback onButtonClicked;

        public ScanDialog()
        {
            InitializeComponent();
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            onButtonClicked(sender, e);
        }


    }
}
