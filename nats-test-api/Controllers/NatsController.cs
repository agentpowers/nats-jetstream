using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NATS.Client;
using STAN.Client;

namespace nats_test_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NatsController : ControllerBase
    {
        private readonly IConnection _natsConnection;
        private readonly IStanConnection _stanConnection;

        public NatsController(IConnection natsConnection, IStanConnection stanConnection)
        {
            _natsConnection = natsConnection;
            _stanConnection = stanConnection;
        }


        [HttpGet("publish")]
        public string Publish([FromQuery]string key, [FromQuery]string value, [FromQuery]int count = 1)
        {
            Stopwatch sw = sw = Stopwatch.StartNew();

            for (int i = 0; i < count; i++)
            {
                _natsConnection.Publish(key, Encoding.UTF8.GetBytes(value));
            }

            sw.Stop();

            Console.WriteLine("Took " + sw.ElapsedMilliseconds);

            return sw.ElapsedMilliseconds.ToString();
        }

        [HttpGet("publish-persisted")]
        public async Task<string> PublishPersisted([FromQuery]string key, [FromQuery]string value, [FromQuery]int count = 1)
        {
            Stopwatch sw = sw = Stopwatch.StartNew();

            for (int i = 0; i < count; i++)
            {
                var resp = await _stanConnection.PublishAsync(key, Encoding.UTF8.GetBytes(value));
            }

            sw.Stop();

            Console.WriteLine("Took " + sw.ElapsedMilliseconds);

            return sw.ElapsedMilliseconds.ToString();
        }
    }
}
