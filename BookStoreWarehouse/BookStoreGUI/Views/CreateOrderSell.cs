using BookStoreWarehouse.Models;
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

        private StoreBook book;
        private string Request;
        public CreateOrderSell(StoreBook book,String Request)
        {
            InitializeComponent();
            this.book = book;
            this.Request = Request;
            this.Text = Request;
            this.textBox4.Text = book.title;
            this.textBox4.Font = new Font("Arial",12 , FontStyle.Bold);
            this.textBox4.Enabled = false;
        }

        public CreateOrderSell()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.book_amount.Value >= 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you really want to buy " + book.title + "?", "Dispath Order", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    User u = new User(this.user_email.Text, this.user_name.Text, this.user_address.Text);
                    Book b = new Book(book.title,0,"",0,book.price,0);
                    Order o = new Order(u,b, Convert.ToInt32(this.book_amount.Value),"");
                    StoreInfo.Instance.dispatchOrder(o, Request);
                    this.Close();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }else
            {
                MessageBox.Show("Please, input a valid book number");
            }
                    
            
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
