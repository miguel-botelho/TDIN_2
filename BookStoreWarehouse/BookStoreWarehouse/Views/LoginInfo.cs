using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookStoreWarehouse.Views
{
    public partial class LoginInfo : Form
    {
        public LoginInfo()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.control_label.Text == "OFFLINE")
            {
                this.control_label.Text = "ONLINE";
                this.control_label.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                this.control_label.Text = "OFFLINE";
                this.control_label.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void control_label_Click(object sender, EventArgs e)
        {

        }
    }
}
