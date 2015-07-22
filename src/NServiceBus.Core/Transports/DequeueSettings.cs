namespace NServiceBus.Transports
{
    /// <summary>
    /// Contains information necessary to set up a message pump for receiving messages.
    /// </summary>
    public class DequeueSettings
    {
        /// <summary>
        /// Creates an instance of <see cref="DequeueSettings"/>.
        /// </summary>
        /// <param name="transportAddress">The transport address of the listener.</param>
        /// <param name="purgeOnStartup"><code>true</code> to purge queue at startup.</param>
        public DequeueSettings(string transportAddress, bool purgeOnStartup)
        {
            Guard.AgainstNull(transportAddress, "transportAddress");
            PurgeOnStartup = purgeOnStartup;
            TransportAddress = transportAddress;
        }

        /// <summary>
        /// The logical address of the listener.
        /// </summary>
        public string TransportAddress { get; private set; }

        /// <summary>
        /// Tells the dequeuer if the queue should be purged before starting to consume messages from it.
        /// </summary>
        public bool PurgeOnStartup { get; private set; }
    }
}