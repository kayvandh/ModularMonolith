namespace ModularMonolith.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CacheInvalidateAttribute : Attribute
    {
        public string Prefix { get; }

        public CacheInvalidateAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }   
}
