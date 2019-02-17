using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NServiceBus.AspNetCore.Services;
using System;

namespace NServiceBus.AspNetCore
{
    /// <summary>
    /// Adds NSB extension methods to <see cref="IServiceCollection"/>.
    /// </summary>
    public static class NsbServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an NSB Endpoint to the service collection. Can be called multiple times to add multiple endpoints to the microservice. Will use Configuration["NServiceBusLicense"] for contents of License.xml if exists.
        /// </summary>
        /// <param name="services">The service collection to add NSB services to.</param>
        /// <param name="endpointName">The name of the NSB endpoint.</param>
        /// <param name="connectionString">The connection string to the transport (currently the RabbitMQ connection string).</param>
        /// <param name="setupAction">Configuration callback for manual settings, including routing.</param>
        public static void AddNServiceBusEndpoint(
            this IServiceCollection services,
            string endpointName,
            string connectionString,
            Action<EndpointConfiguration> setupAction = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrEmpty(endpointName))
                throw new ArgumentNullException(nameof(endpointName));
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            services.TryAddSingleton<IMessageSessionProvider, MessageSessionProvider>();
            services.TryAddSingleton(x => x.GetRequiredService<IMessageSessionProvider>().GetMessageSession()); //default IMessageSession provider
            services.TryAddSingleton<IIncomingNsbMessageContextAccessor, IncomingNsbMessageContextAccessor>();
            services.TryAddTransient<INsbContext, NsbContext>();

            services.AddTransient(serviceProvider =>
            {
                var epConfig = NsbEndpointConfigFactory.Create(endpointName, connectionString, services, serviceProvider, setupAction);

                var container = new NsbConfigContainer()
                {
                    EndpointName = endpointName,
                    EPConfig = epConfig
                };

                return container;
            });
        }
    }
}
