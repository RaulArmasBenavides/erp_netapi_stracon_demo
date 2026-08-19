using FluentValidation;
using SupplierServiceNet.CrossCutting.Supplier;

namespace SupplierServiceNet.Application.Validators
{
    public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
    {
        public CreateSupplierDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is invalid");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("Phone format is invalid");

            RuleFor(x => x.Address)
                .MaximumLength(250).WithMessage("Address must not exceed 250 characters");
        }
    }
}
