using System;

namespace NServiceBus.AspNetCore
{
    class NsbConfigContainer
    {
        public NsbConfigContainer(string endpointName, Func<EndpointConfiguration> endpointConfigurationFactory)
        {
            EndpointName = endpointName;
            EndpointConfigurationFactory = endpointConfigurationFactory;
        }

        public Func<EndpointConfiguration> EndpointConfigurationFactory { get; private set; }

        public string EndpointName { get; private set; }
    }
}
