using BookStoreWarehouse.Models;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;


namespace BookStoreGUI.Models
{
    class SocketClientServer
    {
        Socket socket = IO.Socket("http://172.30.28.65:3000");


        public void initServices()
        {
            socket.On("AvailableBooks", (data) =>
            {
                string json = (string)data;
                List<StoreBook> books = JsonConvert.DeserializeObject<List<StoreBook>>(json);
                StoreInfo.Instance.refreshAvailableBooks(books);
            });
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

        public void refreshAvailableBooks()
        {
            socket.Emit("AvailableBooks","asdas");
        }

        public void createOrder(Order order)
        {
            string json = JsonConvert.SerializeObject(order);
            socket.Emit("NewOrder",json);
            //throw new NotImplementedException();
        }
    }
}
