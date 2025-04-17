using Inventory.Application.Interfaces.Repositories;
using Inventory.Application.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Application.QueryHandlers
{
    public class GetStockHandler : IRequestHandler<GetStockQuery, FluentResults.Result<int>>
    {
        private readonly IStockRepository _stockRepository;

        public GetStockHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<FluentResults.Result<int>> Handle(GetStockQuery request, CancellationToken cancellationToken)
        {
            var result = await _stockRepository.GetStockAsync(request.ProductId);
            return FluentResults.Result.Ok(result);
        }
    }
}
