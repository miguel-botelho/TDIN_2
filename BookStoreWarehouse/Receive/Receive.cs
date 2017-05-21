using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receive
{
    class Receive
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Channel { get; set; }
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public Receive(string HostName,string UserName,string Password,string Channel)
        {
            this.HostName = HostName;
            this.UserName = UserName;
            this.Password = Password;
            this.Channel = Channel;
            
        }

        public void start()
        {
            connection = factory.CreateConnection();
            factory = new ConnectionFactory() { HostName = HostName, Password = Password, UserName = UserName };
            channel = connection.CreateModel();
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

                Console.WriteLine(" [x] Received {0}", order.OrderCode);
            };
            channel.BasicConsume(queue: Channel,
                                 noAck: true,
                                 consumer: consumer);
        }


        
        static void Main(string[] args)
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

                    Console.WriteLine(" [x] Received {0}", order.OrderCode);
                };
                channel.BasicConsume(queue: "order",
                                     noAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        
    }
}
