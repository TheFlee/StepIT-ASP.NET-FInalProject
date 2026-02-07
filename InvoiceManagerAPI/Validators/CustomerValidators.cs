using FluentValidation;
using InvoiceManagerAPI.DTOs;

namespace InvoiceManagerAPI.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequestDTO>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required")
            .MinimumLength(2).WithMessage("Customer name must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must be less than 20 characters");

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("Address must be less than 200 characters");
    }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequestDTO>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required")
            .MinimumLength(2).WithMessage("Customer name must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must be less than 20 characters");

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("Address must be less than 200 characters");
    }
}

public class CustomerQueryParamsValidator : AbstractValidator<CustomerQueryParams>
{
    public CustomerQueryParamsValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SortDirection)
            .Must(x => string.IsNullOrEmpty(x) || x == "asc" || x == "desc")
            .WithMessage("SortDirection must be either 'asc' or 'desc'");
    }
}