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
        /// Starts all configured NServiceBus endpoints.
        /// Also configured NSB Logging to use Microsoft's <see cref="ILogger"/>.
        /// </summary>
        /// <param name="app"></param>
        public static void UseNServiceBus(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            //get all pre-configured configs
            var configs = app.ApplicationServices.GetServices<NsbConfigContainer>().ToArray();

            if (!configs.Any())
                throw new InvalidOperationException("No NServiceBus endpoints defined.");

            var duplicateEndpointName = configs
                .GroupBy(x => x.EndpointName)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .FirstOrDefault();

            if (duplicateEndpointName != null)
                throw new InvalidOperationException($"More than one endpoint with name '{duplicateEndpointName}' has been defined.");

            UseILogger(app.ApplicationServices);

            if (!(app.ApplicationServices.GetRequiredService<IMessageSessionProvider>() is MessageSessionProvider provider))
                throw new InvalidOperationException("IMessageSessionProvider service is not of expected type MessageSessionProvider.");

            foreach (var config in configs)
            {
                var endpointConfiguration = config.EndpointConfigurationFactory();

                var instance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                provider.RegisterEndpointInstance(instance, config.EndpointName);
            }
        }

        private static void UseILogger(IServiceProvider services)
        {
            //set up NServiceBus logging to use Microsoft ILogger, if it exists.

            var loggerFactory = services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            if (loggerFactory != null)
            {
                LogManager.Use<MicrosoftLogFactory>().UseMsFactory(loggerFactory);
            }
        }
    }
}
