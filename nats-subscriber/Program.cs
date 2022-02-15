using Microsoft.AspNetCore.Builder;
using nats_subscriber;
using Microsoft.Extensions.DependencyInjection;

// namespace nats_subscriber
// {
//     class Program
//     {
//         static async Task Main(string[] args)
//         {
//             await Task.Run(Start);
//             var host = new HostBuilder()
//                 .Build();

//             host.Run();
//         }


//         private static async Task Start()
//         {
//             // Create a new connection factory to create
//             // a connection.
//             ConnectionFactory cf = new ConnectionFactory();

//             // Creates a live connection to the default
//             // NATS Server running locally
//             IConnection c = cf.CreateConnection();

//             IJetStream js = c.CreateJetStreamContext();

//             PullSubscribeOptions options = PullSubscribeOptions.Builder()
//                 .WithDurable("durable-name-is-required")
//                 .Build();

//             IJetStreamPullSubscription sub = js.PullSubscribe("test.*", options);

//             var message = sub.Fetch(1, 1000); // 100 messages, 1000 millis timeout
//             foreach(var m in message)
//             {
//                 // process message
//                 m.Ack();
//             }
//         }
//     }
// }

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<ReceiverService>();
var app = builder.Build();

app.Run();
