using FluentValidation;
using Project.Application.Orders.PlaceCustomerOrder;

namespace Project.Application.Orders.PlaceCustomerOrder
{
    public class PlaceCustomerOrderCommandValidator : AbstractValidator<PlaceCustomerOrderCommand>
    {
        public PlaceCustomerOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is empty");
            RuleFor(x => x.Products).NotEmpty().WithMessage("Products list is empty");
            RuleForEach(x => x.Products).SetValidator(new ProductDtoValidator());

            this.RuleFor(x => x.Currency).Must(x => x == "USD" || x == "EUR")
                .WithMessage("At least one product has invalid currency");
        }
    }
}