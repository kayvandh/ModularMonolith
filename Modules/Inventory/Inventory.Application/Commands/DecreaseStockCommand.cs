using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Commands
{
    public class DecreaseStockCommand : IRequest<FluentResults.Result>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
