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
    public partial class PendingPast : Form
    {
        private List<Order> orders = new List<Order>();
        public PendingPast(string name,List<Order> orders,Boolean button)
        {
            
            this.orders = orders;
            InitializeComponent();
            this.button1.Enabled = button;
            this.Text = name;
            loadOrders();
        }

        private void loadOrders()
        {
            foreach (Order o in orders)
            {
                ListViewItem item = new ListViewItem(new string[] { o.OrderCode,o.numBooks.ToString(),o.book.Name,o.user.email, o.status, });
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices;
            if (index.Count > 0)
            {
                ListViewItem item = listView1.Items[index[0]];
                DialogResult dialogResult = MessageBox.Show("Do you really want to dispatch " + item.Text + " to client?", "Dispatch Order", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    StoreInfo.Instance.dispatchOrderStore(item.Text);
                    StoreInfo.Instance.deleteOrderStore(item.Text);
                    BeginInvoke((Action)(() => listView1.Items.Remove(item)));
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
        }
    }
}
