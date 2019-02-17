using System.Threading;

namespace NServiceBus.AspNetCore.Services
{
    class IncomingNsbMessageContextAccessor : IIncomingNsbMessageContextAccessor
    {
        private static AsyncLocal<NsbContextHolder> _contextCurrent = new AsyncLocal<NsbContextHolder>();

        public IMessageProcessingContext Context
        {
            get => _contextCurrent.Value?.Context;
            internal set
            {
                var holder = _contextCurrent.Value;
                if (holder != null)
                {
                    // Clear current HttpContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the IMessageHandlerContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _contextCurrent.Value = new NsbContextHolder { Context = value };
                }
            }
        }

        private class NsbContextHolder
        {
            public IMessageProcessingContext Context;
        }
    }
}
