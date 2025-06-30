namespace ModularMonolith.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CachedResponseAttribute : Attribute
    {
        public int DurationSeconds { get; }

        public CachedResponseAttribute(int durationSeconds = 0)
        {
            DurationSeconds = durationSeconds;
        }
    }

}
