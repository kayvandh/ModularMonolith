using Inventory.Application.Commands;
using Inventory.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Application.CommandHandlers
{
    public class DecreaseStockHandler : IRequestHandler<DecreaseStockCommand, FluentResults.Result>
    {
        private readonly IStockRepository _stockRepository;

        public DecreaseStockHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<FluentResults.Result> Handle(DecreaseStockCommand request, CancellationToken cancellationToken)
        {
            var result = await _stockRepository.DecreaseStockAsync(request.ProductId, request.Quantity);
            if (!result)
            {
                return FluentResults.Result.Fail("Not enough stock.");
            }

            return FluentResults.Result.Ok();
        }
    }
}
