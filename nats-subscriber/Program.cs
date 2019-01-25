using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NATS.Client;
using STAN.Client;
using Microsoft.Extensions.Hosting;

namespace nats_subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            // Creates a live connection to the default
            // NATS Server running locally
            IConnection c = GetConnection();

            EventHandler<MsgHandlerEventArgs> h = (sender, msgArgs) =>
            {
                var now = DateTime.Now;
                var message = msgArgs.Message;
                var arrivalSub = message.ArrivalSubcription;
                // print the message
                Console.WriteLine($"Memory:{now.ToString("u")},Subj={message.Subject},Data={Encoding.UTF8.GetString(message.Data)},ArrivalSub={arrivalSub.Subject}|{arrivalSub.QueuedMessageCount}|{arrivalSub.Queue}");
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            IAsyncSubscription s = c.SubscribeAsync("*", h);

            IStanConnection sc = GetStanConnection();

            EventHandler<StanMsgHandlerArgs> sh = (sender, msgArgs) =>
            {
                var now = DateTime.Now;
                var message = msgArgs.Message;
                // print the message
                Console.WriteLine($"Persisted:{now.ToString("u")},Subj={message.Subject},Data={Encoding.UTF8.GetString(message.Data)}");
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            IStanSubscription ss = sc.Subscribe("key", sh);

            var host = new HostBuilder()
                .Build();

            host.Run();
        }

        public static string NatsUrl()
        {
            // var natsClusterHost = Environment.GetEnvironmentVariable("EXAMPLE_NATS_CLUSTER_SERVICE_HOST");
            // var natsClusterPort = Environment.GetEnvironmentVariable("EXAMPLE_NATS_CLUSTER_SERVICE_PORT");
            // return $"nats://{natsClusterHost}:{natsClusterPort}";
            return Defaults.Url;
        }

        public static IConnection GetConnection()
        {
            var cf = new ConnectionFactory();
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = NatsUrl();

            // Creates a live connection to the default
            // NATS Server running locally
            return cf.CreateConnection(opts);
        }

        public static IStanConnection GetStanConnection()
        {
            var cf = new StanConnectionFactory();
            var opts = StanOptions.GetDefaultOptions();
            opts.NatsURL = NatsUrl();

            // Creates a live connection to the default
            // NATS Server running locally
            return cf.CreateConnection("test-cluster", "nats-subscriber-api", opts);
        }
    }
}
