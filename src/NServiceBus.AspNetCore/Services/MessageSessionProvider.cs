using System;
using System.Collections.Generic;

namespace NServiceBus.AspNetCore.Services
{
    //Provides access to all endpoints in the AspNetCore service provider.

    class MessageSessionProvider : IDisposable, IMessageSessionProvider
    {
        private readonly IDictionary<string, IEndpointInstance> _dictionary = new Dictionary<string, IEndpointInstance>();

        IEndpointInstance _first;

        public void Dispose()
        {
            foreach (var ep in _dictionary.Values)
                ep.Stop().GetAwaiter().GetResult();
        }

        public IMessageSession GetMessageSession(string endpointName)
        {
            return _dictionary[endpointName];
        }

        public IMessageSession GetMessageSession()
        {
            return _first;
        }

        public void RegisterEndpointInstance(IEndpointInstance endpointInstance, string endpointName)
        {
            if (_first == null)
                _first = endpointInstance;

            _dictionary.Add(endpointName, endpointInstance);
        }
    }
}
