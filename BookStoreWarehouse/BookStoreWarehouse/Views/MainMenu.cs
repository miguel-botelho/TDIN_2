using BookStoreWarehouse.Models;
using BookStoreWarehouse.ModelViews;
using BookStoreWarehouse.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookStoreWarehouse
{
    public partial class MainMenu : Form
    {

        private static MainMenu instance;

        public static MainMenu Instance
        {
            get
            {
                if (instance == null)
                    instance = new MainMenu();
                return instance;
            }
        }



        public MainMenu()
        {
            InitializeComponent();
            setupListView();
        }

        private void setupListView()
        {
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.MultiSelect = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientInfo.Instance.click();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoginInfo n = new LoginInfo();
            n.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListViewItem item1 = new ListViewItem(new[] { "sasdasdasdasdasdasdasd1","s2","s3","s4"});
            var item2 = new ListViewItem(new[] { "sasdasdasdasdasdasdasd1", "s2sasdasdasdasdasdasdasd1", "s3" });
            //item1.
            listView1.Items.Add(item1);
            listView1.Items.Add(item2);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void itemClick(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices;
            if(index.Count > 0)
            {
                MessageBox.Show("SELECTED: " + index[0]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DispatchOrder o = new DispatchOrder("cenas");
            o.ShowDialog();
        }
    }
}
