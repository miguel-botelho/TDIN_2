using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public delegate void AlterDelegate(Operation op, Object obj);

public enum Operation { NewPendingOrder,NewPastOrder,DeletePendingOrder,DeletePastOrder,RefreshAvailableBooks };


public interface IClientMessage
{
    void notifyNewPendingOrder(Order order);
    void notifyNewPastOrder(Order order);
}


public interface ISingleServer
{
    event AlterDelegate alterEvent;

    List<Order> getPendingOrders();
    void SomeCall(Guid guid);
    void testLog();
    void addPendingOrder(Order order);
    void addPastOrder(Order order);
    void RefreshServer();
    void dispatchOrder(Order order);
    void deletePendingOrder(Order o);
    List<Order> getPastOrders();
}

public class SingleServer : MarshalByRefObject, ISingleServer
{
    volatile List<Order> pastOrders = new List<Order>();
    volatile List<Order> pendingOrders = new List<Order>();
    Storage storage = new Storage();
    //SEND
    ConnectionFactory factorySend = new ConnectionFactory() { HostName = "172.30.28.65", Password = "tdin", UserName = "tdin" };
    IConnection connectionSend;
    IModel channelSend;


    public SingleServer()
    {
        List<Order> temp1 = storage.LoadObject<List<Order>>("pendingOrders.bin");
        List<Order> temp2 = storage.LoadObject<List<Order>>("pastOrders.bin");
        if (temp1 != null)
            pendingOrders = temp1;
        if(temp2 != null)
            pastOrders = temp2;

        //RECEIVE
        Thread thread1 = new Thread(() =>
        Receive()
        );
        thread1.Start();


        //SEND
        connectionSend = factorySend.CreateConnection();
        channelSend = connectionSend.CreateModel();
    }

    private void Send(string orderID)
    {

        channelSend.QueueDeclare(queue: "orderWarehouse",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var body = Encoding.UTF8.GetBytes(orderID);

        channelSend.BasicPublish(exchange: "",
                                 routingKey: "orderWarehouse",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", orderID);
        
    }

    private void Receive()
    {
        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Password = "tdin", UserName = "tdin" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "order",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                string json = @message;
                Order order = JsonConvert.DeserializeObject<Order>(json);
                addPendingOrder(order);
                Console.WriteLine(" [x] Received {0}", order.OrderCode);
                Console.WriteLine("Name: "+order.book.Name);
            };
            channel.BasicConsume(queue: "order",
                                 noAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    public event AlterDelegate alterEvent;

    public void addPendingOrder(Order order)
    {
        pendingOrders.Add(order);
        Console.WriteLine("TOTAL: " + pendingOrders.Count);
        Console.WriteLine("[PendingOrder] Pending order: " + order.OrderCode);
        NotifyClients(Operation.NewPendingOrder, order);
        storage.SaveObject<List<Order>>(pendingOrders, "pendingOrders.bin");
    }

    public void addPastOrder(Order order)
    {
        pastOrders.Add(order);
        Console.WriteLine("[PastOrders] PastOrder order: " + order.OrderCode);
        NotifyClients(Operation.NewPastOrder, order);
        storage.SaveObject<List<Order>>(pastOrders, "pastOrders.bin");
    }



    public void testLog()
    {
        Console.WriteLine("testlog!");
    }

    public List<Order> getPendingOrders()
    {
        return pendingOrders;
    }

    public void SomeCall(Guid guid)
    {
        //IClientMessage rem = (IClientMessage)RemotingServices.Connect(typeof(IClientMessage), (string)onlineUsers.getUserAdress(guid)); // Obtain a reference to the client remote object
        //Console.WriteLine("[SingleServer]: Obtained the client remote object");
        //rem.SomeMessage("Server calling Client");
    }

    void NotifyClients(Operation op, Object obj)
    {
        if (alterEvent != null)
        {
            Delegate[] invkList = alterEvent.GetInvocationList();

            foreach (AlterDelegate handler in invkList)
            {
                new Thread(() =>
                {
                    try
                    {
                        handler(op, obj);
                        Console.WriteLine("Invoking event handler");
                    }
                    catch (Exception)
                    {
                        alterEvent -= handler;
                        Console.WriteLine("Exception: Removed an event handler");
                    }
                }).Start();
            }
        }
    }

    public void RefreshServer()
    {
        //call to init server objects
    }

    public void dispatchOrder(Order order)
    {
        Send(order.OrderCode);
        addPastOrder(order);
        deletePendingOrder(order);
    }

    public void deletePendingOrder(Order o)
    {
        for (int i = pendingOrders.Count - 1; i >= 0; i--)
        {
            if (pendingOrders[i].OrderCode == o.OrderCode)
                pendingOrders.RemoveAt(i);
        }

        Console.WriteLine("ORDER NO: " + o.OrderCode);
        storage.SaveObject<List<Order>>(pendingOrders, "pendingOrders.bin");
        NotifyClients(Operation.DeletePendingOrder, o);
        Console.WriteLine("PENDING: "+pendingOrders.Count+" PAST: "+pastOrders.Count);
    }

    public List<Order> getPastOrders()
    {
        return pastOrders;
    }
}

public class AlterEventRepeater : MarshalByRefObject
{
    public event AlterDelegate alterEvent;

    public override object InitializeLifetimeService()
    {
        return null;
    }

    public void Repeater(Operation op, Object obj)
    {
        if (alterEvent != null)
            alterEvent(op, obj);
    }
}

[Serializable]
public class User
{
    public string email { get; set; }
    public string name { get; set; }
    public string address { get; set; }

    public User()
    {

    }

    public User(string email, string name, string address)
    {
        this.email = email;
        this.name = name;
        this.address = address;
    }
}

[Serializable]
public class Order
{
    public User user { get; set; }
    public Book book { get; set; }
    public int numBooks { get; set; }
    public String OrderCode { get; set; }
    public string status { get; set; }

    public Order()
    {

    }
    public Order(User user,Book book,int numBooks,string status)
    {
        this.user = user;
        this.book = book;
        this.numBooks = numBooks;
        this.status = status;
    }
}

[Serializable]
public class StoreBook
{
    public String title { get; set; }
    public double price { get; set; }
    public int stock { get; set; }

    public StoreBook()
    {

    }

    public StoreBook(String title, double price, int stock)
    {
        this.title = title;
        this.stock = stock;
        this.price = price;
    }
}

[Serializable]
public class Book
{
    public String Name { get; set; }
    public int PageNumber { get; set; }
    public String Author { get; set; }
    public int Stars { get; set; }
    public double Price { get; set; }
    public int ISBN { get; set; }

    public Book()
    {

    }

    public Book(String Name, int PageNumber, String Author, int Stars, double Price, int ISBN)
    {
        this.Name = Name;
        this.PageNumber = PageNumber;
        this.Author = Author;
        this.Stars = Stars;
        this.Price = Price;
        this.ISBN = ISBN;
    }
}

class Receive
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Channel { get; set; }

    public Receive(string HostName, string UserName, string Password, string Channel)
    {
        this.HostName = HostName;
        this.UserName = UserName;
        this.Password = Password;
        this.Channel = Channel;
    }

    public void start(Action<Order> addPendingOrder)
    {
        ConnectionFactory factory = new ConnectionFactory() { HostName = HostName, Password = Password, UserName = UserName };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: Channel,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                string json = @message;
                Order order = JsonConvert.DeserializeObject<Order>(json);
                addPendingOrder(order);
                Console.WriteLine(" [x] Received {0}", order.OrderCode);
            };

            /*channel.BasicConsume(queue: Channel,
                                 noAck: true,
                                 consumer: consumer);*/

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}