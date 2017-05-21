using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookStoreGUI
{
    public partial class CreateOrderSell : Form
    {


        public CreateOrderSell(Book book,String Request)
        {
            InitializeComponent();
            this.Text = Request;
            this.textBox4.Text = book.Name;
            this.textBox4.Font = new Font("Arial",12 , FontStyle.Bold);
            this.textBox4.Enabled = false;
        }

        public CreateOrderSell()
        {
            InitializeComponent();
            this.Text="ok";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Buy books?");
            Guid orderGUID = System.Guid.NewGuid();
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
