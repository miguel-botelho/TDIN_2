using BookStoreWarehouse.Models;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;


namespace BookStoreGUI.Models
{
    class SocketClientServer
    {
        Socket socket = IO.Socket("http://localhost:3000");


        public void initServices()
        {
            socket.On("AvailableBooks", (data) =>
            {
                string json = (string)data;
                List<StoreBook> books = JsonConvert.DeserializeObject<List<StoreBook>>(json);
                StoreInfo.Instance.refreshAvailableBooks(books);
            });

            socket.On("newOrder", (data) =>
            {
                string json = (string)data;
                Order order = JsonConvert.DeserializeObject<Order>(json);
                StoreInfo.Instance.updateStockBook(order);
            });

            socket.On("ordersWarehouse", (data) =>
            {
                string json = (string)data;
                List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);
                StoreInfo.Instance.updateOrdersWarehouse(orders);
            });
            socket.On("ordersStore", (data) =>
            {
                string json = (string)data;
                List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);
                StoreInfo.Instance.updateOrdersStore(orders);
            });
        }

        internal void refreshOrderWarehouse()
        {
            socket.Emit("ordersWarehouse", "");
        }

        public void start()
        {
            

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("test");
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
            socket.Emit("AvailableBooks","");
        }

        public void refreshOrdersStore()
        {
            socket.Emit("ordersStore", "");
        }

        internal void dispatchOrderStore(string text)
        {
            socket.Emit("accept",text);
        }

        public void createOrder(Order order)
        {
            string json = JsonConvert.SerializeObject(order);
            socket.Emit("sell",json);
        }

        internal void createSell(Order order)
        {
            string json = JsonConvert.SerializeObject(order);
            socket.Emit("directSell", json);
        }
    }
}
