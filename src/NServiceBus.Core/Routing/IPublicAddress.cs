namespace NServiceBus.Features.Routing
{
    /// <summary>
    /// Allows to access the public address advertised by an endpoint.
    /// </summary>
    public interface IPublicAddress
    {
        /// <summary>
        /// Gets the public transport address.
        /// </summary>
        string TransportAddress { get; }
    }
}