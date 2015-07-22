namespace NServiceBus.Features.DelayedDelivery
{
    using System;

    /// <summary>
    /// Allows to configure the timeout address.
    /// </summary>
    public class TimeoutManagerAddressConfiguration : ITimeoutManagerAddress
    {
        string timeoutManagerAddress;

        internal TimeoutManagerAddressConfiguration(string defaultTimeoutManagerAddress)
        {
            timeoutManagerAddress = defaultTimeoutManagerAddress;
        }

        /// <summary>
        /// Sets the address of the timeout manager.
        /// </summary>
        public void Set(string newTimeoutManagerAddress)
        {
            Guard.AgainstNullAndEmpty(newTimeoutManagerAddress, "newTimeoutManagerAddress");
            if (timeoutManagerAddress != null)
            {
                throw new InvalidOperationException("Another feature or the UnicastBusConfig section has already set the timeout manager address.");
            }
            timeoutManagerAddress = newTimeoutManagerAddress;
        }

        string ITimeoutManagerAddress.TransportAddress { get { return timeoutManagerAddress; } }
    }
}