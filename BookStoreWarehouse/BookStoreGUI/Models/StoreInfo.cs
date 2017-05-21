using BookStoreGUI.Models;
using System;
using System.Collections.Generic;

namespace BookStoreWarehouse.Models
{
    class StoreInfo
    {

        public List<Order> pendingOrders { get; set; }
        public List<Order> pastOrders { get; set; }
        public List<StoreBook> availableBooks = new List<StoreBook>();
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

               
        public void dispatchOrder(Order order)
        {
            throw new NotImplementedException();
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
