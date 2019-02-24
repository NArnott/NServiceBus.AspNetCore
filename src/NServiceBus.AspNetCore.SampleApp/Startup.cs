using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.AspNetCore.SampleApp.BusMessages;
using NServiceBus.AspNetCore.SampleApp.Stores;
using Swashbuckle.AspNetCore.Swagger;

namespace NServiceBus.AspNetCore.SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public string Endpoint1Name { get; } = "MyEndpoint1";

        public string Endpoint2Name { get; } = "MyEndpoint2";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<MemoryStore>();

            services.AddMvcCore().AddApiExplorer();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info()
                {
                    Title = "TestApi"
                });
            });

            services
                .AddNServiceBus()
                //add NSB Endpoint 1
                .AddNsbEndpoint(Endpoint1Name, endpointConfiguration =>
                {
                    var transport = endpointConfiguration.UseTransport<LearningTransport>();

                    transport.Routing().RouteToEndpoint(typeof(TestCommand), Endpoint1Name);
                })
                //add NSB Endpoint 2
                .AddNsbEndpoint(Endpoint2Name, endpointConfiguration =>
                {
                    var transport = endpointConfiguration.UseTransport<LearningTransport>();

                    transport.Routing().RouteToEndpoint(typeof(TestCommand), Endpoint2Name);
                })
                .AddGlobalEndpointPostConfigurator(x =>
                {
                    //configure all endpoints here
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint($"../swagger/v1/swagger.json", "V1");
            });
            app.UseMvc();

            //start NSB endpoint.
            app.UseNServiceBus();
        }
    }
}
