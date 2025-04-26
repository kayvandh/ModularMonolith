using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance
{
    public class UnitOfWorkOptions
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = new();

    }
}
