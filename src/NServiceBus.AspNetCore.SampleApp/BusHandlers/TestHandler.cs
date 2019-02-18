using System;
using System.Threading.Tasks;
using NServiceBus.AspNetCore.SampleApp.BusMessages;
using NServiceBus.AspNetCore.SampleApp.Stores;

namespace NServiceBus.AspNetCore.SampleApp.BusHandlers
{
    class TestHandler :
        IHandleMessages<TestCommand>,
        IHandleMessages<TestValueChangedEvent>
    {
        private readonly MemoryStore _store;

        //Inject MemoryStore, a service defined in Startup.ConfigureServices.
        public TestHandler(MemoryStore store)
        {
            _store = store;
        }

        async Task IHandleMessages<TestCommand>.Handle(TestCommand message, IMessageHandlerContext context)
        {
            if (message.Crash)
                throw new Exception("Test Crash");

            await _store.SetValueAsync(message.TestValue);
        }

        Task IHandleMessages<TestValueChangedEvent>.Handle(TestValueChangedEvent message, IMessageHandlerContext context)
        {
            //do nothing

            return Task.CompletedTask;
        }
    }
}
