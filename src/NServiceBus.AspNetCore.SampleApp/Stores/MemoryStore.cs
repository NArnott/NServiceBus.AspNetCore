using NServiceBus.AspNetCore.SampleApp.BusMessages;
using NServiceBus.AspNetCore.Services;
using System;
using System.Threading.Tasks;

namespace NServiceBus.AspNetCore.SampleApp.Stores
{
    public class MemoryStore : IDisposable
    {
        static string _persistedValue;

        private readonly INsbContext _nsb;

        //Inject INsbContext, allowing us to send messages on NServiceBus regardless if it's an AspNetCore request or NServiceBus message without the need for passing around NSB contexts.
        public MemoryStore(INsbContext nsb)
        {
            _nsb = nsb;
        }

        public async Task SetValueAsync(string newValue)
        {
            await _nsb.Publish(new TestValueChangedEvent() { OldValue = _persistedValue, NewValue = newValue });

            _persistedValue = newValue;
        }

        public string GetValue()
        {
            return _persistedValue;
        }

        public void Dispose()
        {
            //do nothing
        }
    }
}
