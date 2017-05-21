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


        public static ClientInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientInfo();
                return instance;
            }
        }

        internal void click()
        {
            server.testLog();
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

        internal void InitServer()
        {
            server.InitServer();
        }

        public void initChat(Guid guid, string address)
        {
            IClientMessage cli = (IClientMessage)RemotingServices.Connect(typeof(IClientMessage), address);
        }

        internal void addNewPendingOrder(Order order)
        {
           
        }

        internal void dispatchOrder(Order order)
        {
            server.dispatchOrder(order);
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
            throw new NotImplementedException();
        }

        public void notifyNewPendingOrder(Order order)
        {
            ClientInfo.Instance.addNewPendingOrder(order);
        }
    }
}
