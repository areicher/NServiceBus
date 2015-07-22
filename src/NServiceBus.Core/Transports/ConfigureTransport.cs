namespace NServiceBus.Transports
{
    using System;
    using Features;
    using NServiceBus.ObjectBuilder;
    using NServiceBus.Pipeline;
    using Unicast.Transport;

    /// <summary>
    /// Base class for configuring <see cref="TransportDefinition"/> features.
    /// </summary>
    public abstract class ConfigureTransport : Feature
    {
        /// <summary>
        ///  Initializes a new instance of <see cref="ConfigureTransport"/>.
        /// </summary>
        protected ConfigureTransport()
        {
            Defaults(s => s.SetDefault<TransportConnectionString>(TransportConnectionString.Default));
            
        }

        /// <summary>
        /// <see cref="Feature.Setup"/>.
        /// </summary>
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            var selectedTransportDefinition = context.Settings.Get<TransportDefinition>();
            context.Settings.Get<QueueBindings>().BindReceiving(selectedTransportDefinition.CreateInputQueueTransportAddress(context.Settings.RootLogicalAddress()));
            var connectionString = context.Settings.Get<TransportConnectionString>().GetConnectionStringOrNull();

            if (connectionString == null && RequiresConnectionString)
            {
                throw new InvalidOperationException(String.Format(Message, GetConfigFileIfExists(), selectedTransportDefinition.GetType().Name, ExampleConnectionStringForErrorMessage));
            }

            context.Container.RegisterSingleton(selectedTransportDefinition);

            if (!context.Settings.Get<bool>("Endpoint.SendOnly"))
            {
                var receiveBehaviorFactory = GetReceiveBehaviorFactory(new ReceiveOptions(context.Settings));
                context.RegisterReceiveBehavior(receiveBehaviorFactory);
            }
            Configure(context, connectionString);
        }

        /// <summary>
        /// Creates a <see cref="RegisterStep"/> for receive behavior.
        /// </summary>
        protected abstract Func<IBuilder, ReceiveBehavior> GetReceiveBehaviorFactory(ReceiveOptions receiveOptions);

        /// <summary>
        /// Gives the chance to implementers to set themselves up.
        /// </summary>
        protected abstract void Configure(FeatureConfigurationContext context, string connectionString);

        /// <summary>
        /// Used by implementations to provide an example connection string that till be used for the possible exception thrown if the <see cref="RequiresConnectionString"/> requirement is not met.
        /// </summary>
        protected abstract string ExampleConnectionStringForErrorMessage { get; }

        /// <summary>
        /// Used by implementations to control if a connection string is necessary.
        /// </summary>
        /// <remarks>If this is true and a connection string is not returned by <see cref="TransportConnectionString.GetConnectionStringOrNull"/> then an exception will be thrown.</remarks>
        protected virtual bool RequiresConnectionString
        {
            get { return true; }
        }

        static string GetConfigFileIfExists()
        {
            return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile ?? "App.config";
        }

        const string Message =
            @"No default connection string found in your config file ({0}) for the {1} Transport.

To run NServiceBus with {1} Transport you need to specify the database connectionstring.
Here is an example of what is required:
  
  <connectionStrings>
    <add name=""NServiceBus/Transport"" connectionString=""{2}"" />
  </connectionStrings>";

    }
}
