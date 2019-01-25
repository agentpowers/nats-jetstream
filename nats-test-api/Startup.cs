using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using STAN.Client;

namespace nats_test_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IConnection>((_) => GetConnection());
            services.AddSingleton<IStanConnection>((_) => GetStanConnection());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        public string NatsUrl()
        {
            // var natsClusterHost = Environment.GetEnvironmentVariable("EXAMPLE_NATS_CLUSTER_SERVICE_HOST");
            // var natsClusterPort = Environment.GetEnvironmentVariable("EXAMPLE_NATS_CLUSTER_SERVICE_PORT");
            // return $"nats://{natsClusterHost}:{natsClusterPort}";
            return Defaults.Url;
        }

        public IConnection GetConnection()
        {
            var cf = new ConnectionFactory();
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = NatsUrl();

            // Creates a live connection to the default
            // NATS Server running locally
            return cf.CreateConnection(opts);
        }

        public IStanConnection GetStanConnection()
        {
            var cf = new StanConnectionFactory();
            var opts = StanOptions.GetDefaultOptions();
            opts.NatsURL = NatsUrl();

            // Creates a live connection to the default
            // NATS Server running locally
            return cf.CreateConnection("test-cluster", "nats-test-api", opts);
        }
    }
}
