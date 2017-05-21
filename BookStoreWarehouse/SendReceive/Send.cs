using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Send
{
    class Send
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string channel { get; set; }



        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "172.30.28.65",Password="tdin",UserName="tdin" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orderWarehouse",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "a9ef17bc-5986-4e45-808b-0c8c9349861d";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "orderWarehouse",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
