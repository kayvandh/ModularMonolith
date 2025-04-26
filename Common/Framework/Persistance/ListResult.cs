using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance
{
    public class ListResult<T>
    {
        public required IReadOnlyList<T> Items { get; set; }
        public int TotalCount { get; set; }

    }
}
