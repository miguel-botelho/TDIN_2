using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrinterGUI
{
    public partial class Printer : Form
    {

        private static Printer instance;

        public static Printer Instance
        {
            get
            {
                if (instance == null)
                    instance = new Printer();
                return instance;
            }
        }


        public Printer()
        {
            InitializeComponent();
        }

        public void newOrder(string name, string address, string email, string bookName, int numbooks)
        {
            BeginInvoke((Action)(() => printOrder(name, address,email,bookName,numbooks)));
        }

        private void printOrder(string name,string address,string email,string bookName,int numbooks)
        {
            this.richTextBox1.AppendText("PRINTING ORDER OF: "+name+" ADDRESS: "+ address +" EMAIL: "+email+" BOOK: "+bookName+" NUM BOOKS: "+numbooks);
            this.richTextBox1.AppendText("\r\n");
            this.richTextBox1.ScrollToCaret();
        }
    }
}
