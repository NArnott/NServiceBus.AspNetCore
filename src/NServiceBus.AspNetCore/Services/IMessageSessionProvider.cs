namespace NServiceBus.AspNetCore.Services
{
    /// <summary>
    /// Provides access to <see cref="IMessageSession"/> by NSB endpoint name.
    /// </summary>
    public interface IMessageSessionProvider
    {
        /// <summary>
        /// Returns the <see cref="IMessageSession"/> by NSB endpoint name.
        /// </summary>
        /// <param name="endpointName"></param>
        /// <returns></returns>
        IMessageSession GetMessageSession(string endpointName);

        /// <summary>
        /// Returns the first IMessageSession registered with the provider.
        /// </summary>
        /// <returns></returns>
        IMessageSession GetMessageSession();
    }
}