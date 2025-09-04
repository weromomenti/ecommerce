using FluentValidation;
using OrderService.Entities;

namespace OrderService.Infrustructure
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(request => request.ProductName)
                .NotEmpty().WithMessage("Product name cannot be empty.")
                .Length(1, 100).WithMessage("Product name must be between 1 and 100 characters.");
            RuleFor(request => request.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
