using BookStoreWarehouse.Models;
using BookStoreWarehouse.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BookStoreWarehouse
{
    public partial class MainMenu : Form
    {

        AlterEventRepeater evRepeater;
        delegate ListViewItem LVAddDelegate(ListViewItem lvOrder);
        delegate void ChCommDelegate(Object obj);
        private Hashtable pendingOrders = new Hashtable();
        private Hashtable pastOrders = new Hashtable();
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
            ClientInfo.Instance.InitServer();
            InitializeComponent();
            setupListView();
            loadPendingOrders();
        }

        private void loadPendingOrders()
        {
            List<Order> orders = ClientInfo.Instance.getPendingOrders();
            foreach (Order o in orders)
            {
                pendingOrders.Add(o.OrderCode,o);
                ListViewItem item1 = new ListViewItem(new[] { o.OrderCode, o.book.Name,o.book.Price.ToString(), o.numBooks.ToString(), o.user.name });
                listView1.Items.Add(item1);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
                    lvAdd = new LVAddDelegate(listView1.Items.Add);
                    pendingOrders.Add(((Order)obj).OrderCode, (Order)obj);
                    ListViewItem lvUsr = new ListViewItem(new string[] { ((Order)obj).OrderCode, ((Order)obj).book.Name, ((Order)obj).book.Price.ToString(), ((Order)obj).numBooks.ToString(),((Order)obj).user.name });
                    lvUsr.Tag = ((Order)obj);
                    BeginInvoke(lvAdd, new object[] { lvUsr });
                    break;
                case Operation.NewPastOrder:
                    if (!pastOrders.ContainsKey(((Order)obj).OrderCode))
                    {
                        pastOrders.Add(((Order)obj).OrderCode, (Order)obj);
                    }
                    break;
                case Operation.DeletePendingOrder:
                    chComm = new ChCommDelegate(DeleteOrder);
                    if (pendingOrders.ContainsKey(((Order)obj).OrderCode))
                    {
                        pendingOrders.Remove(((Order)obj).OrderCode);
                        BeginInvoke(chComm, new object[] { (Order)obj });
                    }
                    break;
            }
        }

        private void DeleteOrder(object obj)
        {
            Order o = (Order)obj;
            foreach (ListViewItem lvU in listView1.Items)
            {
                if (lvU.SubItems[0].Text == o.OrderCode)
                {
                    lvU.Remove();
                }
            }
        }

        private void setupListView()
        {
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.MultiSelect = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            ClientInfo.server.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientInfo.Instance.InitServer();
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
            listView1.Items.Add(item1);
            listView1.Items.Add(item2);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        private void onCLose(object sender, FormClosedEventArgs e)
        {
            ClientInfo.server.alterEvent -= new AlterDelegate(evRepeater.Repeater);
            evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices;
            if (index.Count > 0)
            {
                ListViewItem item = listView1.Items[index[0]];
                DialogResult dialogResult = MessageBox.Show("Do you really want to dispatch "+item.Text+"?","Dispath Order", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                   
                   Order o = (Order)pendingOrders[item.Text];
                   DoAlterations(Operation.NewPastOrder, o);
                   DoAlterations(Operation.DeletePendingOrder, o);

                    BeginInvoke((Action)(() => ClientInfo.Instance.dispatchOrder(o)));
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.button5.Enabled = false;
            listView1.Items.Clear();
            loadOrders(pendingOrders);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.button5.Enabled = true;
            listView1.Items.Clear();
            loadOrders(pastOrders);
        }

        private void loadOrders(Hashtable orders)
        {
            foreach(Order o in orders)
            {
                ListViewItem item1 = new ListViewItem(new[] { o.OrderCode, o.book.Name, o.book.Price.ToString(), o.numBooks.ToString(), o.user.name });
                listView1.Items.Add(item1);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        
    }
}
