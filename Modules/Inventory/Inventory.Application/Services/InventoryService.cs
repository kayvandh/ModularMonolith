using Inventory.Application.Commands;
using Inventory.Application.Queries;
using MediatR;
using ModularMonolith.Contract.Inventory.Interfaces;
using System;
using System.Threading.Tasks;

namespace Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IMediator _mediator;

        public InventoryService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<FluentResults.Result> DecreaseStockAsync(Guid productId, int quantity)
        {
            return await _mediator.Send(new DecreaseStockCommand { ProductId = productId, Quantity = quantity });
        }

        public async Task<FluentResults.Result<int>> GetStockAsync(Guid productId)
        {
            return await _mediator.Send(new GetStockQuery { ProductId = productId });
        }
    }
}
