using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookStoreGUI.Models
{
    class SocketClientServer
    {
        Socket socket = IO.Socket("http://172.30.28.65:3000");


        public void Send(string s)
        {
            socket.Emit("sell","caralho");
        }
       

        public void start()
        {
            

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("caralho");
            });

            socket.On("newOrder", (data) =>
            {
                Console.WriteLine(data);
                socket.Disconnect();
            });
            Console.ReadLine();
        }
    }
}
