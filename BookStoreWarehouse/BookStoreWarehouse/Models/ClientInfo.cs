using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private ClientInfo()
        {

        }
        //REMOTING MESSAGING

        public void receiveMessage(string s)
        {
            Console.WriteLine("Message received!");
        }

        public void initChat(Guid guid, string address)
        {
            IClientMessage cli = (IClientMessage)RemotingServices.Connect(typeof(IClientMessage), address);
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

    public class Chats
    {
        public Hashtable chat_clients = new Hashtable();
        public Hashtable chat_messages = new Hashtable();
        public Hashtable chat_window = new Hashtable();

        public Chats()
        {

        }
        public void addNewChat(IClientMessage cli, Guid guid, string username)
        {
            chat_clients.Add(guid, cli);
            //chat_window.Add(guid, new Chat(guid, username));
        }

        public bool isChattingWith(User user)
        {
            if (chat_clients.ContainsKey(user.guid))
                return true;
            else
                return false;
        }

        public IClientMessage getRemoteClient(Guid guid)
        {
            return (IClientMessage)chat_clients[guid];
        }

        public void removeChat(Guid guid)
        {
            chat_clients.Remove(guid);
            chat_messages.Remove(guid);
            chat_window.Remove(guid);
        }
        public void addMessage(Guid guid, string message)
        {
            string temp = chat_messages[guid] + "\n";
            chat_messages[guid] = temp + message;
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
            throw new NotImplementedException();
        }
    }
}
