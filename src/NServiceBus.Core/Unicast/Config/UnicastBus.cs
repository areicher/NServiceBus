namespace NServiceBus.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.Config;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;
    using NServiceBus.Settings;
    using NServiceBus.Settings.Concurrency;
    using NServiceBus.Settings.Throttling;
    using NServiceBus.Transports;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Messages;
    using TransactionSettings = NServiceBus.Unicast.Transport.TransactionSettings;

    class UnicastBus : Feature
    {
        internal UnicastBus()
        {
            EnableByDefault();

            Defaults(s =>
            {
                s.SetDefault<IConcurrencyConfig>(new SharedConcurrencyConfig(null));
                s.SetDefault<IThrottlingConfig>(new NoLimitThrottlingConfig());
                var section = s.GetConfigSection<UnicastBusConfig>();
                if (section.TimeoutManagerAddress != null)
                {
                    s.Set("TimeoutManagerAddress", section.TimeoutManagerAddress);
                }
            });

            Defaults(s =>
            {
                var endpointInstanceName = GetEndpointInstanceName(s);
                var rootLogicalAddress = new LogicalAddress(endpointInstanceName);
                s.SetDefault<EndpointInstanceName>(endpointInstanceName);
                s.SetDefault<LogicalAddress>(rootLogicalAddress);
            });
        }

        static EndpointInstanceName GetEndpointInstanceName(ReadOnlySettings settings)
        {
            var userDiscriminator = settings.GetOrDefault<string>("EndpointInstanceDiscriminator");
            var transportDiscriminator = settings.Get<TransportDefinition>().GetDiscriminatorForThisEndpointInstance();
            if (userDiscriminator == null && transportDiscriminator == null)
            {
                throw new Exception("No endpoint instance discriminator found. This value is usually provided by your transport so please make sure you're on the lastest version of your specific transport or set the discriminator using 'configuration.ScaleOut().UniqueQueuePerEndpointInstance(myDiscriminator)'");
            }
            return new EndpointInstanceName(settings.EndpointName(), userDiscriminator, transportDiscriminator);
        }

        protected internal override void Setup(FeatureConfigurationContext context)
        {
            context.Container.ConfigureComponent<BusNotifications>(DependencyLifecycle.SingleInstance);


            var concurrencyConfig = context.Settings.Get<IConcurrencyConfig>();
            var throttlingConfig = context.Settings.Get<IThrottlingConfig>();

            var transportConfig = context.Settings.GetConfigSection<TransportConfig>();

            if (transportConfig != null)
            {
                if (transportConfig.MaximumConcurrencyLevel != 0)
                {
                    concurrencyConfig = new SharedConcurrencyConfig(transportConfig.MaximumConcurrencyLevel);
                }
                if (transportConfig.MaximumMessageThroughputPerSecond == 0)
                {
                    throttlingConfig = new NoLimitThrottlingConfig();
                }
                else if (transportConfig.MaximumMessageThroughputPerSecond != -1)
                {
                    throttlingConfig = new SharedLimitThrottlingConfig(transportConfig.MaximumConcurrencyLevel);
                }
            }

            context.Container.ConfigureComponent(b => throttlingConfig.WrapExecutor(concurrencyConfig.BuildExecutor()), DependencyLifecycle.SingleInstance);

            context.Container.ConfigureComponent<BehaviorContextStacker>(DependencyLifecycle.SingleInstance);

            context.Container.ConfigureComponent(b => b.Build<BehaviorContextStacker>().GetCurrentOrRootContext(), DependencyLifecycle.InstancePerCall);

            //Hack because we can't register as IStartableBus because it would automatically register as IBus and overrode the proper IBus registration.
            context.Container.ConfigureComponent<UnicastBusInternal>(DependencyLifecycle.SingleInstance);

            var knownMessages = context.Settings.GetAvailableTypes()
                .Where(context.Settings.Get<Conventions>().IsMessageType)
                .ToList();

            ConfigureMessageRegistry(context, knownMessages);

            if (context.Settings.GetOrDefault<bool>("Endpoint.SendOnly"))
            {
                return;
            }

            HardcodedPipelineSteps.RegisterIncomingCoreBehaviors(context.Pipeline);

            var transactionSettings = new TransactionSettings(context.Settings);

            if (transactionSettings.DoNotWrapHandlersExecutionInATransactionScope)
            {
                context.Pipeline.Register<SuppressAmbientTransactionBehavior.Registration>();
            }
            else
            {
                context.Pipeline.Register<HandlerTransactionScopeWrapperBehavior.Registration>();
            }
        }

        static void ConfigureMessageRegistry(FeatureConfigurationContext context, IEnumerable<Type> knownMessages)
        {
            var messageRegistry = new MessageMetadataRegistry(context.Settings.Get<Conventions>());

            foreach (var msg in knownMessages)
            {
                messageRegistry.RegisterMessageType(msg);
            }

            context.Container.RegisterSingleton(messageRegistry);
            context.Container.ConfigureComponent<LogicalMessageFactory>(DependencyLifecycle.SingleInstance);

            if (!Logger.IsInfoEnabled)
            {
                return;
            }

            var messageDefinitions = messageRegistry.GetAllMessages().ToList();

            Logger.InfoFormat("Number of messages found: {0}", messageDefinitions.Count());

            if (!Logger.IsDebugEnabled)
            {
                return;
            }

            Logger.DebugFormat("Message definitions: \n {0}",
                string.Concat(messageDefinitions.Select(md => md.ToString() + "\n")));
        }

        static ILog Logger = LogManager.GetLogger<UnicastBus>();
    }
}