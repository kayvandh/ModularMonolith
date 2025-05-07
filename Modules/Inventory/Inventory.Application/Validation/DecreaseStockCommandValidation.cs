using FluentValidation;
using Inventory.Application.Commands;

namespace Inventory.Application.Validation
{
    public class DecreaseStockCommandValidation : AbstractValidator<DecreaseStockCommand>
    {
        public DecreaseStockCommandValidation()
        {
            RuleFor(p => p.Quantity)
                .NotEmpty().WithMessage(Common.Resource.Message.Required)
                .GreaterThan(0).WithMessage(Common.Resource.Message.GreaterThan);
        }
    }
}
