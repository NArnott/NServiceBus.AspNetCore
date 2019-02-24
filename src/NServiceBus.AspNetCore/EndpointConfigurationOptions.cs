using System;
using System.Collections.Generic;

namespace NServiceBus.AspNetCore
{
    class EndpointConfigurationOptions
    {
        public List<Action<EndpointConfiguration>> GlobalPreConfigurators { get; } = new List<Action<EndpointConfiguration>>();

        public List<Action<EndpointConfiguration>> GlobalPostConfigurators { get; } = new List<Action<EndpointConfiguration>>();
    }
}
