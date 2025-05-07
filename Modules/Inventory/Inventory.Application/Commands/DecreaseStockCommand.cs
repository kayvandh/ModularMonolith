using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Application.Commands
{
    public class DecreaseStockCommand : IRequest<FluentResults.Result>
    {
        [Display(Name = nameof(ProductId), ResourceType = typeof(Common.Resource.PropertyName))]
        public Guid ProductId { get; set; }

        [Display(Name = nameof(Quantity) , ResourceType = typeof(Common.Resource.PropertyName))]
        public int Quantity { get; set; }
    }
}
