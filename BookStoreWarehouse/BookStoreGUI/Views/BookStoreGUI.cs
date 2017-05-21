using BookStoreGUI.Models;
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
        public BookStoreGUI()
        {
            InitializeComponent();
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
            Sell s = new Sell();
            s.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SocketClientServer s = new SocketClientServer();
            s.Send("asdasd");
        }
    }
}
