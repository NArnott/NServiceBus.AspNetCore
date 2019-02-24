using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus.AspNetCore.Behaviors;
using System;

namespace NServiceBus.AspNetCore
{
    class EndpointConfigurationFactory
    {
        internal static EndpointConfiguration Create(
            string endpointName,
            IServiceCollection serviceCollection,
            IServiceProvider serviceProvider,
            Action<EndpointConfiguration> configureEndpoint)
        {
            var epConfig = new EndpointConfiguration(endpointName);

            //Try to set license
            SetLicense(epConfig, serviceProvider);

            //configure NSB container to use existing Microsoft Depdenency Injection
            epConfig.UseContainer<ServicesBuilder>(x =>
            {
                x.ExistingServices(serviceCollection);
            });

            AddCustomBehaviors(epConfig);

            var configOptions = serviceProvider.GetService<IOptions<EndpointConfigurationOptions>>()?.Value;

            if (configOptions != null)
            {
                foreach (var configurator in configOptions.GlobalPreConfigurators)
                    configurator(epConfig);
            }

            configureEndpoint?.Invoke(epConfig);

            if (configOptions != null)
            {
                foreach (var configurator in configOptions.GlobalPostConfigurators)
                    configurator(epConfig);
            }

            return epConfig;
        }

        private static void AddCustomBehaviors(EndpointConfiguration epConfig)
        {
            //add custom behaviors
            epConfig.Pipeline.Register(typeof(IncomingNsbMessageContextBehavior), "Adds support for IIncomingNsbMessageContextAccessor.");
        }

        private static void SetLicense(EndpointConfiguration epConfig, IServiceProvider services)
        {
            var logger = services.GetService<ILogger<EndpointConfigurationFactory>>();

            var config = services.GetService<IConfiguration>();
            var licenseText = config?["NServiceBusLicenseText"];

            if (!string.IsNullOrWhiteSpace(licenseText))
            {
                epConfig.License(licenseText);

                logger?.LogInformation("NServiceBus license was found from IConfiguration[\"NServiceBusLicenseText\"].");
            }
            else
            {
                logger?.LogWarning("NServiceBus license was not found from IConfiguration[\"NServiceBusLicenseText\"], and will not automatically be set.");
            }
        }
    }
}
