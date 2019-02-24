using Microsoft.Extensions.DependencyInjection;
using System;

namespace NServiceBus.AspNetCore
{
    /// <summary>
    /// Used to configure NServiceBus
    /// </summary>
    public class NsbBuilder
    {
        internal NsbBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// The services being configured.
        /// </summary>
        public IServiceCollection Services { get; }

        #region Add Nsb Endpoint

        /// <summary>
        /// Adds an NSB Endpoint. Can be called multiple times to add multiple endpoints to the microservice. Will use Configuration["NServiceBusLicense"] for contents of License.xml if exists.
        /// </summary>
        /// <param name="endpointName">The name of the NSB endpoint.</param>
        /// <returns>The builder.</returns>
        public NsbBuilder AddNsbEndpoint(string endpointName)
        {
            if (string.IsNullOrEmpty(endpointName))
                throw new ArgumentNullException(nameof(endpointName));

            AddNsbEndpointHelper(endpointName, null);

            return this;
        }

        /// <summary>
        /// Adds an NSB Endpoint. Can be called multiple times to add multiple endpoints to the microservice. Will use Configuration["NServiceBusLicense"] for contents of License.xml if exists.
        /// </summary>
        /// <param name="endpointName">The name of the NSB endpoint.</param>
        /// <param name="configureEndpoint">Configuration callback for manual settings, including routing.</param>
        /// <returns>The builder.</returns>
        public NsbBuilder AddNsbEndpoint(string endpointName, Action<EndpointConfiguration> configureEndpoint)
        {
            if (string.IsNullOrEmpty(endpointName))
                throw new ArgumentNullException(nameof(endpointName));
            if (configureEndpoint == null)
                throw new ArgumentNullException(nameof(configureEndpoint));

            AddNsbEndpointHelper(endpointName, configureEndpoint);

            return this;
        }

        private void AddNsbEndpointHelper(string endpointName, Action<EndpointConfiguration> configureEndpoint)
        {
            Services.AddTransient(serviceProvider =>
            {
                var container = new NsbConfigContainer(
                    endpointName,
                    endpointConfigurationFactory: () => EndpointConfigurationFactory.Create(endpointName, Services, serviceProvider, configureEndpoint)
                );

                return container;
            });
        }

        #endregion

        /// <summary>
        /// Adds a global NSB endpoint configuration callback that occurs prior to the endpoint's primary configuration callback.
        /// </summary>
        public NsbBuilder AddGlobalEndpointPreConfigurator(Action<EndpointConfiguration> configureEndpoint)
        {
            if (configureEndpoint == null)
                throw new ArgumentNullException(nameof(configureEndpoint));

            Services.Configure<EndpointConfigurationOptions>(x =>
            {
                x.GlobalPreConfigurators.Add(configureEndpoint);
            });

            return this;
        }

        /// <summary>
        /// Adds a global NSB endpoint configuration callback that occurs after the endpoint's primary configuration callback.
        /// </summary>
        public NsbBuilder AddGlobalEndpointPostConfigurator(Action<EndpointConfiguration> configureEndpoint)
        {
            if (configureEndpoint == null)
                throw new ArgumentNullException(nameof(configureEndpoint));

            Services.Configure<EndpointConfigurationOptions>(x =>
            {
                x.GlobalPostConfigurators.Add(configureEndpoint);
            });

            return this;
        }

    }
}
