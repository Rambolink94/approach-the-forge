namespace ApproachTheForge.Utility.Upgrade
{
    public class UpgradeWrapper<T>
        where T : IUpgrade
    {
        public T Upgrade { get; set; }

        public int Cost { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}
