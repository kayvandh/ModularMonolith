using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Queries
{
    public class GetStockQuery : IRequest<FluentResults.Result<int>>
    {
        public Guid ProductId { get; set; }
    }
}
