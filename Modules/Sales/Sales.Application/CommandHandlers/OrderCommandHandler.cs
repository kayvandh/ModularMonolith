using FluentResults;
using MediatR;
using Sales.Application.Commands;
using Sales.Domain;
using Sales.Application.Interfaces.Repositories;
using Common.Contract.Inventory.Interfaces;

namespace Sales.Application.CommandHandlers
{
    public class OrderCommandHandler : IRequestHandler<OrderCommand, Result>
    {
        private readonly IInventoryService inventoryService;  
        private readonly IOrderRepository orderRepository;

        public OrderCommandHandler(IInventoryService inventoryService, IOrderRepository orderRepository)
        {
            inventoryService = inventoryService;
            orderRepository = orderRepository;
        }

        public async Task<Result> Handle(OrderCommand request, CancellationToken cancellationToken)
        {
            var stockResult = await inventoryService.GetStockAsync(request.ProductId);

            if (stockResult.IsSuccess && stockResult.Value >= request.Quantity)
            {
                var order = new Order
                {
                    Quantity = stockResult.Value,
                    ProductId = request.ProductId,
                    Customer = request.Customer,
                    Id = request.Id,
                    OrderDate = request.OrderDate
                };

                await orderRepository.AddOrderAsync(order);

                return Result.Ok();
            }

            return Result.Fail("Insufficient stock to fulfill the order.");
        }
    }
}
