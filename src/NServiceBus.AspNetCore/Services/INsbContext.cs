namespace NServiceBus.AspNetCore.Services
{
    /// <summary>
    /// Provides a general way to Send, Publish and Reply to NSB messages without having pass various contexts around.
    /// </summary>
    public interface INsbContext : IMessageProcessingContext
    {
    }
}
