using BookStoreGUI.Models;
using System;
using System.Collections.Generic;

namespace BookStoreWarehouse.Models
{
    class StoreInfo
    {

        public List<Order> pendingOrders1 { get; set; }
        public List<Order> pastOrders { get; set; }
        public List<StoreBook> availableBooks = new List<StoreBook>();
        public List<Order> ordersStore = new List<Order>();
        public List<Order> ordersWarehouse = new List<Order>();

        public SocketClientServer service = new SocketClientServer();

        private static StoreInfo instance;
    
        public static StoreInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new StoreInfo();
                return instance;
            }
        }


        private StoreInfo()
        {
            service.initServices();
            loadData();
        }

        internal void loadData()
        {
            //THIS FUNCTION LOADS ALL THE NECESSARY INFO IN THE SERVER
            service.refreshAvailableBooks();
            service.refreshOrdersStore();
            service.refreshOrderWarehouse();
        }

        internal void updateStockBook(Order order)
        {
            foreach(StoreBook b in availableBooks)
            {
                if(b.title == order.book.Name)
                {
                    b.stock -= order.numBooks;
                }
            }
            BookStoreGUI.BookStoreGUI.Instance.DoAlterations(Operation.RefreshAvailableBooks,availableBooks);
        }

        internal void updateOrdersStore(List<Order> orders)
        {
            this.ordersStore = orders;
        }

        internal void updateOrdersWarehouse(List<Order> orders)
        {
            this.ordersWarehouse = orders;
        }

        internal void dispatchOrderStore(string text)
        {
            service.dispatchOrderStore(text);
        }

        internal void refreshAvailableBooks(List<StoreBook> books)
        {
            this.availableBooks = books;
            BookStoreGUI.BookStoreGUI.Instance.refreshAvailableBooks();
        }

        public List<Order> getPendingOrders()
        {
            throw new NotImplementedException();
        }

        public StoreBook getBookByName(string name)
        {
            foreach(StoreBook b in availableBooks)
            {
                if (b.title == name)
                    return b;
            }
            return null;
        }

        public void deleteOrderStore(string orderid)
        {
            for(int i = 0; i < ordersStore.Count; i++)
            {
                if (ordersStore[i].OrderCode == orderid)
                {
                    Console.WriteLine("pls");
                    ordersStore.RemoveAt(i);
                    break;
                }
            }
        }
               
        public void dispatchOrder(Order order,string Request)
        {
            if(Request == "Sell")
            {

                int x = -1;
                for(int i = 0; i < StoreInfo.Instance.availableBooks.Count; i++)
                {
                    if(order.book.Name == StoreInfo.Instance.availableBooks[i].title)
                    {
                        x = i;
                    }
                }
                
                service.createSell(order);
                if (x != -1)
                    StoreInfo.Instance.availableBooks[x].stock -= order.numBooks;
                //BookStoreGUI.BookStoreGUI.Instance.refreshAvailableBooks();

                BookStoreGUI.BookStoreGUI.Instance.RefreshAvailableBooks(StoreInfo.Instance.availableBooks);
                //BeginBookStoreGUI.BookStoreGUI.Instance
            }
            else
            {
                service.createOrder(order);
            }
        }

        internal List<Order> getPastOrders()
        {
            throw new NotImplementedException();
        }

        public void refreshAvailableBooks()
        {
            service.refreshAvailableBooks();
        }
    }
}
