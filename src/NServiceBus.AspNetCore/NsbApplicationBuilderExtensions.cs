using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus.AspNetCore.Services;
using NServiceBus.Logging;
using System;
using System.Linq;

namespace NServiceBus.AspNetCore
{
    /// <summary>
    /// Adds NSB extension methods to <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderNsbExtensions
    {
        /// <summary>
        /// Starts the NSB Endpoints added using <see cref="NsbServiceCollectionExtensions.AddNServiceBusEndpoint(IServiceCollection, string, string, Action{EndpointConfiguration})"/>.
        /// Also configured NSB Logging to use Microsoft's <see cref="ILogger"/>.
        /// </summary>
        /// <param name="app"></param>
        public static void UseNServiceBusEndpoints(this IApplicationBuilder app)
        {
            //get all pre-configured configs
            var configs = app.ApplicationServices.GetServices<NsbConfigContainer>();

            if (!configs.Any())
                throw new InvalidOperationException("UseNServiceBusEndpoints first requires a call to NsbServiceCollectionExtensions.AddNServiceBusEndpoint.");


            //set up NServiceBus logging to use Microsoft ILogger
            var logFactory = LogManager.Use<MicrosoftLogFactory>();
            logFactory.UseMsFactory(app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>());



            if (!(app.ApplicationServices.GetRequiredService<IMessageSessionProvider>() is MessageSessionProvider provider))
                throw new InvalidOperationException("IMessageSessionProvider service is not of expected type MessageSessionProvider.");

            foreach (var config in configs)
            {
                var instance = Endpoint.Start(config.EPConfig).GetAwaiter().GetResult();

                provider.RegisterEndpointInstance(instance, config.EndpointName);
            }
        }

    }
}
