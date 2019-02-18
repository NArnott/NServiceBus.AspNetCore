namespace NServiceBus.AspNetCore.Services
{
    /// <summary>
    /// Provides a general way to Send, Publish and Reply to NSB messages without having pass various contexts around.
    /// </summary>
    public interface INsbContext : IMessageProcessingContext
    {
        /// <summary>
        /// Returns true if currently in an NSB Pipeline request. Returns false if in an AspNetCore request.
        /// </summary>
        bool IsInNsbPipeline { get; }
    }
}
