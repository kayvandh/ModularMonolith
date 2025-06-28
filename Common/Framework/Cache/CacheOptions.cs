using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Cache
{
    public class CacheOptions
    {
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(30);
    }
}
