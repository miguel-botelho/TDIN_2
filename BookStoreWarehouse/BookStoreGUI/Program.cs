using BookStoreWarehouse.Models;
using PrinterGUI;
using System;
using System.Windows.Forms;

namespace BookStoreGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StoreInfo.Instance.loadData();
            BookStoreGUI gui = BookStoreGUI.Instance;
            Printer p = Printer.Instance;
            p.Show();
            Application.Run(gui);
        }
    }
}
