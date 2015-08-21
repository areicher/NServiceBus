namespace NServiceBus.Transports
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;

    /// <summary>
    /// Defines a transport.
    /// </summary>
    public abstract class TransportDefinition
    {
        /// <summary>
        /// Indicates that the transport is capable of supporting the publish and subscribe pattern natively.
        /// </summary>
        public bool HasNativePubSubSupport { get; protected set; }

        /// <summary>
        /// Indicates that the transport has a central store for subscriptions.
        /// </summary>
        public bool HasSupportForCentralizedPubSub { get; protected set; }

        /// <summary>
        /// Indicates that the transport has support for distributed transactions.
        /// </summary>
        public bool? HasSupportForDistributedTransactions { get; protected set; }

        /// <summary>
        /// Indicates if the transport has support for multi-queue native transactions that allow to receive and send multiple messages atomically.
        /// </summary>
        public bool HasSupportForMultiQueueNativeTransactions { get; protected set; }

        /// <summary>
        /// True if the transport.
        /// </summary>
        public bool RequireOutboxConsent { get; set; }

        /// <summary>
        /// Gives implementations access to the <see cref="BusConfiguration"/> instance at configuration time.
        /// </summary>
        protected internal virtual void Configure(BusConfiguration config)
        {

        }

        /// <summary>
        /// Returns the list of supported delivery constraints for this transport.
        /// </summary>
        public abstract IEnumerable<Type> GetSupportedDeliveryConstraints();

        /// <summary>
        /// Returns the consistency guarantee to use if no specific guarantee is specified.
        /// </summary>
        public abstract ConsistencyGuarantee GetDefaultConsistencyGuarantee();

        /// <summary>
        /// Will be called if the transport has indicated that it has native support for pub sub.
        /// Creates a transport address for the input queue defined by a logical address.
        /// </summary>
        public abstract IManageSubscriptions GetSubscriptionManager();

        /// <summary>
        /// Creates a transport address for the input queue defined by a logical address.
        /// </summary>
        /// <param name="logicalAddress">A logical address for the receiver.</param>
        public abstract string CreateInputQueueTransportAddress(LogicalAddress logicalAddress);
    }
}