using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Dtos
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
    }
}
