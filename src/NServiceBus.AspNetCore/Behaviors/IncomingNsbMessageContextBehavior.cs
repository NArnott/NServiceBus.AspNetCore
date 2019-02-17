using NServiceBus.AspNetCore.Services;
using NServiceBus.Pipeline;
using System;
using System.Threading.Tasks;

namespace NServiceBus.AspNetCore.Behaviors
{
    class IncomingNsbMessageContextBehavior : Behavior<IIncomingPhysicalMessageContext>
    {
        private readonly IncomingNsbMessageContextAccessor _accessor;

        public IncomingNsbMessageContextBehavior(IIncomingNsbMessageContextAccessor accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));

            _accessor = (accessor as IncomingNsbMessageContextAccessor) 
                ?? throw new ArgumentException("IMessageProcessingContextAccessor must be of type NsbContextAccessorBehavior.", nameof(accessor));
        }

        public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            _accessor.Context = context;

            try
            {
                await next().ConfigureAwait(false);
            }
            finally
            {
                _accessor.Context = null;
            }
        }
    }
}
