using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain
{
    public class InventoryRoleEntityTypePermission
    {
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        public int EntityTypeId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanReadOwnOnly { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public Role Role { get; set; } = default!;
        public EntityType EntityType { get; set; } = default!;
    }
}
