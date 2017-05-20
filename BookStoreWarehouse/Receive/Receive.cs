using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receive
{
    class Receive
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string channel { get; set; }



        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Password = "tdin", UserName = "tdin" };
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
                    //string json = JsonConvert.SerializeObject(message);
                    Console.WriteLine(" [x] Received {0}", message);
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
