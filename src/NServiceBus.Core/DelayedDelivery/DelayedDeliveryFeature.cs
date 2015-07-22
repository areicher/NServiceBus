namespace NServiceBus.Features
{
    using NServiceBus.Config;
    using NServiceBus.DelayedDelivery;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Features.DelayedDelivery;
    using NServiceBus.Pipeline;
    using NServiceBus.TransportDispatch;


    class DelayedDeliveryFeature : Feature
    {
        public DelayedDeliveryFeature()
        {
            EnableByDefault();
            DependsOnOptionally<TimeoutManager>();
            Defaults(s =>
            {
                var timeoutManagerAddressConfiguration = new TimeoutManagerAddressConfiguration(s.GetConfigSection<UnicastBusConfig>().TimeoutManagerAddress);
                s.Set<TimeoutManagerAddressConfiguration>(timeoutManagerAddressConfiguration);
                s.Set<ITimeoutManagerAddress>(timeoutManagerAddressConfiguration);
            });
            Prerequisite(c => c.DoesTransportSupportConstraint<DelayedDeliveryConstraint>() || c.Settings.Get<ITimeoutManagerAddress>().TransportAddress != null,
                "Transport does not support delayed delivery constraint and timeout manager has been disabled.");
        }
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            if (!context.DoesTransportSupportConstraint<DelayedDeliveryConstraint>())
            {
                var timeoutManagerAddress = context.Settings.Get<ITimeoutManagerAddress>().TransportAddress;

                context.Pipeline.Register<RouteDeferredMessageToTimeoutManagerBehavior.Registration>();


                context.Container.ConfigureComponent(b => new RouteDeferredMessageToTimeoutManagerBehavior(timeoutManagerAddress), DependencyLifecycle.SingleInstance);


                context.Container.ConfigureComponent(b =>
                {
                    var pipelinesCollection = context.Settings.Get<PipelineConfiguration>();

                    var dispatchPipeline = new PipelineBase<DispatchContext>(b, context.Settings, pipelinesCollection.MainPipeline);

                    return new RequestCancelingOfDeferredMessagesFromTimeoutManager(timeoutManagerAddress, dispatchPipeline);
                }, DependencyLifecycle.SingleInstance);

            }
            else
            {
                context.Container.ConfigureComponent(b => new NoOpCanceling(), DependencyLifecycle.SingleInstance);
            }

            context.Pipeline.Register("ApplyDelayedDeliveryConstraint", typeof(ApplyDelayedDeliveryConstraintBehavior), "Applied relevant delayed delivery constraints requested by the user");
        }
    }
}