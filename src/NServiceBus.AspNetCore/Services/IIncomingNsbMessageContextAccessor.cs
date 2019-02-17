namespace NServiceBus.AspNetCore.Services
{
    /// <summary>
    /// Provides access to the NSB Context for inbound messages.
    /// </summary>
    public interface IIncomingNsbMessageContextAccessor
    {
        /// <summary>
        /// Gets access to the <see cref="IMessageProcessingContext"/> for the current inbound message.
        /// </summary>
        IMessageProcessingContext Context { get; }
    }
}