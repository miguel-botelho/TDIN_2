using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;

namespace BookStoreWarehouse.Models
{
    class StoreInfo
    {

        public List<Order> pendingOrders { get; set; }
        public List<Order> pastOrders { get; set; }
        public List<Book> availableBooks { get; set; }

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

        internal void load()
        {
            //throw new NotImplementedException();
        }

        public List<Order> getPendingOrders()
        {
            throw new NotImplementedException();
        }

        private StoreInfo()
        {

        }
               
        public void dispatchOrder(Order order)
        {
            throw new NotImplementedException();
        }

        internal List<Order> getPastOrders()
        {
            throw new NotImplementedException();
        }

    }
}
