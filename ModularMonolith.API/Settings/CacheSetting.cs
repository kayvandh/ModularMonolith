using Framework.Cache;

namespace ModularMonolith.API.Settings
{
    public class CacheSettings
    {
        public CacheProvider Provider { get; set; }
        public int ResponseCacheDurationSeconds { get; set; }
    }
}
