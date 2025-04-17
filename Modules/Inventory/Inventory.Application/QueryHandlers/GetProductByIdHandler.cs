using AutoMapper;
using FluentResults;
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
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<Dtos.Product>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper mapper;

        public GetProductByIdHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Dtos.Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);
            if (product == null)
                return Result.Fail("محصول مورد نظر یافت نشد.");

            return Result.Ok(mapper.Map<Dtos.Product>(product));
        }
    }
}
