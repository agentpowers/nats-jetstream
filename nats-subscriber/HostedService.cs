using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client;
using NATS.Client.Internals;
using NATS.Client.JetStream;

namespace nats_subscriber
{
    public record Payload(Guid Id, string Message, DateTime Created);

    public class ReceiverService : IHostedService, IDisposable
    {
        private readonly ILogger<ReceiverService> _logger;
        private readonly IJetStream _js;
        private readonly IConnection _c;

        public ReceiverService(ILogger<ReceiverService> logger)
        {
            _logger = logger;
            // Create a new connection factory to create
            // a connection.
            var cf = new ConnectionFactory();

            // Creates a live connection to the default
            // NATS Server running locally
            _c = cf.CreateConnection();

            _js = _c.CreateJetStreamContext();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service running.");

            Task.Run(() => SubscribeMessages(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service is stopping.");

            return Task.CompletedTask;
        }

        private async Task SubscribeMessages(CancellationToken stoppingToken)
        {
            try
            {
                // Create a JetStreamManagement context.
                IJetStreamManagement jsm = _c.CreateJetStreamManagementContext();
                var consumer = jsm.GetConsumerInfo("test", "main");
                ConsumerConfiguration cc = ConsumerConfiguration.Builder(consumer.ConsumerConfiguration)
                        .WithAckWait(2500)
                        .Build();
                jsm.AddOrUpdateConsumer("test", cc);

                PullSubscribeOptions pullOptions = PullSubscribeOptions.Builder()
                    .WithStream("test")
                    .WithDurable("main") // required
                    .WithConfiguration(cc) //TODO: Not working as expected, possible bug
                    .Build();

                var topic = "test.*";

                IJetStreamPullSubscription sub = _js.PullSubscribe(topic, pullOptions);

                while(!stoppingToken.IsCancellationRequested)
                {
                    var message = sub.Fetch(1, 60 * 1000); // 100 messages, 1min
                    _logger.LogInformation("Pulled {ct} messages from {topic}", message.Count, topic);
                    foreach(var m in message)
                    {
                        try
                        {
                            await ProcessMessage(m);
                            m.Ack();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error processing message", ex);
                            m.Nak();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error subscribing message");
            }
        }

        private async Task ProcessMessage(Msg msg)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(5000));
            var payload = System.Text.Json.JsonSerializer.Deserialize<Payload>(msg.Data);
            // if (msgStr.Contains('9'))
            // {
            //     throw new Exception("bad message");
            // }
            _logger.LogInformation("Subject={Subject}, Msg={payload}, Meta={MetaData}", msg.Subject, payload, msg.MetaData);
        }

        public void Dispose()
        {
            _c.Dispose();
        }
    }
}