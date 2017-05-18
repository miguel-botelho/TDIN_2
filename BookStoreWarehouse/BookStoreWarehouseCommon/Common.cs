using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

public delegate void AlterDelegate(Operation op, Object obj);

public enum Operation { NewPendingOrder,NewPastOrder };


public interface IClientMessage
{
    void notifyNewPendingOrder(Order order);
    void notifyNewPastOrder(Order order);
}


public interface ISingleServer
{
    event AlterDelegate alterEvent;

    void SomeCall(Guid guid);
    void testLog();
    void addPendingOrder(Order order);
}

public class SingleServer : MarshalByRefObject, ISingleServer
{
    List<Order> pastOrders;
    List<Order> pendingOrders;


    public event AlterDelegate alterEvent;

    public void addPendingOrder(Order order)
    {
        pendingOrders.Add(order);
        Console.WriteLine("[PendingOrder] Pending order: " + order.OrderCode);
        NotifyClients(Operation.NewPendingOrder, order);
    }

    public void testLog()
    {
        Console.WriteLine("testlog!");
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
    public Guid guid { get; set; }
    public string name { get; set; }
    public string address { get; set; }

    public User() : this(Guid.Empty, "", "")
    {
    }

    public User(Guid guid, string name, string address)
    {
        this.guid = guid;
        this.name = name;
        this.address = address;
    }
}

public class Utils
{
    public static int SERVER_PORT = 9000;

    public static void InvokeFix(Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            control.Invoke(new MethodInvoker(delegate { action(); }));
        }
        else
        {
            action();
        }
    }
}

[Serializable]
public class PublicMessage
{
    public string chatRoom;
    public string sender;
    public string message;

    public PublicMessage(string chatRoom, string sender, string message)
    {
        this.chatRoom = chatRoom;
        this.sender = sender;
        this.message = message;
    }
}

[Serializable]
public class Order
{
    public String user { get; set; }
    public Book book { get; set; }
    public int numBooks { get; set; }
    public String OrderCode { get; set; }
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