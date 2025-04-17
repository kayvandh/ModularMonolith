using FluentResults;
using MediatR;
using System;

namespace Inventory.Application.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<Result<Dtos.Product>>;

}
