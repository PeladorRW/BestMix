namespace BestMix
{
    public class test : RegionProcessorSubtitution
    {
        Verse.RegionProcessor processor;
        void t()
        {
            processor = RegionProcessor;
        }
        protected override bool RegionProcessor(Verse.Region reg)
        {
            return true;
        }
    }
}