namespace NServiceBus.Features.DelayedDelivery
{
    /// <summary>
    /// Allows to access the timeout manager address.
    /// </summary>
    public interface ITimeoutManagerAddress
    {
        /// <summary>
        /// Gets the transport address of the timeout manager for this endpoint.
        /// </summary>
        string TransportAddress { get; }
    }
}