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
        Socket socket2 = IO.Socket("http://localhost:3001");


        public void initServices()
        {
            socket.On("AvailableBooks", (data) =>
            {
                Console.WriteLine("MERDA");
                string json = (string)data;
                List<StoreBook> books = JsonConvert.DeserializeObject<List<StoreBook>>(json);
                StoreInfo.Instance.refreshAvailableBooks(books);
            });
            socket2.On("newOrder", (data) =>
            {
                Console.WriteLine("CARALHO1");
                string json = (string)data;
                Order order = JsonConvert.DeserializeObject<Order>(json);
                StoreInfo.Instance.updateStockBook(order);
            });
            socket.On("newOrder", (data) =>
            {
                Console.WriteLine("CARALHO2");
                string json = (string)data;
                Order order = JsonConvert.DeserializeObject<Order>(json);
                StoreInfo.Instance.updateStockBook(order);
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
            socket.Emit("sell",json);
        }
    }
}
