using BookStoreWarehouse.Models;
using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;

namespace BookStoreWarehouse
{
    static class Launcher
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
            IDictionary props = new Hashtable();
            props["port"] = 0;  // let the system choose a free port
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            TcpChannel chan = new TcpChannel(props, clientProvider, serverProvider);  // instantiate the channel
            ChannelServices.RegisterChannel(chan, false);                             // register the channel

            ChannelDataStore data = (ChannelDataStore)chan.ChannelData;
            int myPort = new Uri(data.ChannelUris[0]).Port;                            // get the port


            Console.WriteLine("port: " + myPort);
            RemotingConfiguration.Configure("BookStoreWarehouse.exe.config", false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ReceiveMessage), "Messages", WellKnownObjectMode.Singleton);  // register my remote object for service

            string address = "tcp://localhost:" + myPort.ToString() + "/Messages";
            ReceiveMessage r = (ReceiveMessage)RemotingServices.Connect(typeof(ReceiveMessage), address);    // connect to the registered my remote object here
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(MainMenu.Instance);
        }
    }
}
