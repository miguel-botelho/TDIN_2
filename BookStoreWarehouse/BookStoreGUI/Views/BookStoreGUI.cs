using BookStoreGUI.Models;
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
    public partial class BookStoreGUI : Form
    {

        AlterEventRepeater evRepeater;
        delegate ListViewItem LVAddDelegate(ListViewItem lvOrder);
        delegate void ChCommDelegate(Object obj);

        private static BookStoreGUI instance;

        public static BookStoreGUI Instance
        {
            get
            {
                if (instance == null)
                    instance = new BookStoreGUI();
                return instance;
            }
        }


        public BookStoreGUI()
        {
            InitializeComponent();
            StoreInfo.Instance.loadData();
        }

        internal void refreshAvailableBooks()
        {
            DoAlterations(Operation.RefreshAvailableBooks,StoreInfo.Instance.availableBooks);
        }

        /* The client is also a remote object. The Server calls remotely the AlterEvent handler  *
       * Infinite lifetime                                                                     */

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /* Event handler for the remote AlterEvent subscription and other auxiliary methods */

        public void DoAlterations(Operation op, Object obj)
        {
            LVAddDelegate lvAdd;
            ChCommDelegate chComm;

            switch (op)
            {
                case Operation.RefreshAvailableBooks:
                    chComm = new ChCommDelegate(RefreshAvailableBooks);
                    BeginInvoke(chComm, new object[] { obj });
                    break;
            }
        }


        private void RefreshAvailableBooks(object obj)
        {
            listView1.Items.Clear();
            List<StoreBook> books = (List<StoreBook>)obj;
            foreach(StoreBook b in books)
            {
                ListViewItem item = new ListViewItem(new string[] {b.title,b.stock.ToString(),b.price.ToString()+"€"});
                listView1.Items.Add(item);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button4_Click(object sender, EventArgs e)
        {
           /* PendingPast p = new PendingPast("WHAT");
            p.ShowDialog();*/
        }

        private void button5_Click(object sender, EventArgs e)
        {
            /*PendingPast p = new PendingPast("asdasd");
            p.ShowDialog();*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices;
            if (index.Count > 0)
            {
                ListViewItem item = listView1.Items[index[0]];
                StoreBook b = StoreInfo.Instance.getBookByName(item.Text);
                CreateOrderSell createOrder = new CreateOrderSell(b, "Sell");
                createOrder.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SocketClientServer s = new SocketClientServer();
            User user = new User("miguel_botelho@gmail.com","Miguel Botelho","Rua dos caralhos");
            Book book = new Book();
            /*Order o = new Order(user,book,20);
            s.createOrder(o);*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices;
            if (index.Count > 0)
            {
                ListViewItem item = listView1.Items[index[0]];
                StoreBook b = StoreInfo.Instance.getBookByName(item.Text);
                CreateOrderSell createOrder = new CreateOrderSell(b, "Create New Order");
                createOrder.ShowDialog();
            }
            
        }

        internal void RefreshAvailableBooks(List<StoreBook> bks)
        {
            listView1.Items.Clear();
            foreach (StoreBook b in bks)
            {
                ListViewItem item = new ListViewItem(new string[] { b.title, b.stock.ToString(), b.price.ToString() + "€" });
                listView1.Items.Add(item);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void BookStoreGUI_Load(object sender, EventArgs e)
        {
            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Total books: "+StoreInfo.Instance.availableBooks.Count);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            StoreInfo.Instance.loadData();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            PendingPast p = new PendingPast("Orders in Store",StoreInfo.Instance.ordersStore,true);
            p.ShowDialog();
            p.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            PendingPast p = new PendingPast("Orders in Warehouse", StoreInfo.Instance.ordersWarehouse, false);
            p.ShowDialog();
            p.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.R)
            {
                StoreInfo.Instance.loadData();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
