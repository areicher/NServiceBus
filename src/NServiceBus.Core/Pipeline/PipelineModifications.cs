namespace NServiceBus.Pipeline
{
    using System.Collections.Generic;

    class PipelineModifications
    {
        public List<RegisterStep> Additions = new List<RegisterStep>();
        public List<RemoveStep> Removals = new List<RemoveStep>();
        public List<ReplaceBehavior> Replacements = new List<ReplaceBehavior>();
    }

    class SatellitePipelineModifications : PipelineModifications
    {
        public readonly string Name;
        public readonly string TransportAddress;

        public SatellitePipelineModifications(string name, string transportAddress)
        {
            Name = name;
            TransportAddress = transportAddress;
        }
    }
}