namespace NServiceBus.Features.Routing
{
    using System;

    /// <summary>
    /// Allows features to override the default public transport address settings.
    /// </summary>
    public class PublicAddressesConfiguration : IPublicAddress
    {
        string transportAddress;
        readonly string defaultTransportAddress;

        internal PublicAddressesConfiguration(string userOverriddenAddress, string defaultTransportAddress)
        {
            transportAddress = userOverriddenAddress;
            this.defaultTransportAddress = defaultTransportAddress;
        }

        string IPublicAddress.TransportAddress
        {
            get { return transportAddress ?? defaultTransportAddress; }
        }

        /// <summary>
        /// Overrides the public transport address.
        /// </summary>
        /// <param name="newTransportAddress">New transport address.</param>
        public void Override(string newTransportAddress)
        {
            Guard.AgainstNullAndEmpty(newTransportAddress, "transportAddress");
            if (transportAddress != null)
            {
                throw new InvalidOperationException("Another feature or the bus configuration code has already overridden the public transport address.");
            }
            transportAddress = newTransportAddress;
        }
    }
}