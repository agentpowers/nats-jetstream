using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NATS.Client;
using NATS.Client.JetStream;
using System.Text.Json;

namespace nats_publisher
{
    public record Payload(Guid Id, string Message, DateTime Created);
    class Program
    {
        static async Task Main(string[] args)
        {
            await Start();
        }

        private static async Task Start()
        {
            // Create a new connection factory to create
            // a connection.
            ConnectionFactory cf = new ConnectionFactory();

            // Creates a live connection to the default
            // NATS Server running locally
            IConnection c = cf.CreateConnection();

            IJetStream js = c.CreateJetStreamContext();

            PublishOptions.PublishOptionsBuilder builder = PublishOptions.Builder()
                .WithExpectedStream("test");

            var count = 10;

            Console.WriteLine($"Started {count} messages");

            for (int i = 0; i < count; i++)
            {
                var payload = JsonSerializer.SerializeToUtf8Bytes(new Payload(Guid.NewGuid(), $"hello {i}", DateTime.UtcNow));
                var msg = new Msg("test.hello", payload);
                await js.PublishAsync(msg, builder.Build());
            }
            
            Console.WriteLine($"Completed {count} messages");
        }
    }
}
