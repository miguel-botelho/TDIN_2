using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;

namespace BookStoreWarehouse.Models
{
    class ClientInfo
    {
        private static ClientInfo instance;
        public static ISingleServer server = (ISingleServer)R.New(typeof(ISingleServer));
        public static Boolean serverStatus = false;

        public static ClientInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientInfo();
                return instance;
            }
        }

        public List<Order> getPendingOrders()
        {
            return server.getPendingOrders();
        }

        private ClientInfo()
        {

        }
        //REMOTING MESSAGING

        public void receiveMessage(string s)
        {
            Console.WriteLine("Message received!");
        }

        internal void RefreshServer()
        {
            server.RefreshServer();
        }

        internal void dispatchOrder(Order order)
        {
            server.dispatchOrder(order);
        }

        internal List<Order> getPastOrders()
        {
            return server.getPastOrders();
        }
    }

    class R
    {
        private static IDictionary wellKnownTypes;

        public static object New(Type type)
        {
            if (wellKnownTypes == null)
                InitTypeCache();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)wellKnownTypes[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return Activator.GetObject(type, entry.ObjectUrl);
        }

        public static void InitTypeCache()
        {
            Hashtable types = new Hashtable();

            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
            {
                if (entry.ObjectType == null)
                    throw new RemotingException("A configured type could not be found!");
                types.Add(entry.ObjectType, entry);
            }
            wellKnownTypes = types;
        }
    }
    public class ReceiveMessage : MarshalByRefObject, IClientMessage
    {

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void notifyNewPastOrder(Order order)
        {

            Console.WriteLine("MEREDASAASDASDAS-2");
            throw new NotImplementedException();
        }

        public void notifyNewPendingOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
