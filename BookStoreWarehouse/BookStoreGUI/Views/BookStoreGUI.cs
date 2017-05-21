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
                case Operation.NewPendingOrder:
                    //lvAdd = new LVAddDelegate(listView1.Items.Add);
                    //pendingOrders.Add(((Order)obj).OrderCode, (Order)obj);
                    //ListViewItem lvUsr = new ListViewItem(new string[] { ((Order)obj).OrderCode, ((Order)obj).book.Name, ((Order)obj).book.Price.ToString(), ((Order)obj).numBooks.ToString(), ((Order)obj).user.name });
                    //lvUsr.Tag = ((Order)obj);
                    //if (button5.Enabled != false)
                     //   BeginInvoke(lvAdd, new object[] { lvUsr });
                    break;
                case Operation.NewPastOrder:
                    //if (!pastOrders.ContainsKey(((Order)obj).OrderCode))
                    //{
                    //    pastOrders.Add(((Order)obj).OrderCode, (Order)obj);
                    //}
                    break;
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
            PendingPast p = new PendingPast();
            p.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            PendingPast p = new PendingPast();
            p.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateOrderSell s = new CreateOrderSell();
            s.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SocketClientServer s = new SocketClientServer();
            User user = new User("miguel_botelho@gmail.com","Miguel Botelho","Rua dos caralhos");
            Book book = new Book();
            Order o = new Order(user,book,20);
            s.createOrder(o);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Book book = new Book();
            CreateOrderSell createOrder = new CreateOrderSell(book,"Create New Order");
            createOrder.ShowDialog();
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
    }
}
