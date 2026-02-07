using FluentValidation;
using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;

namespace InvoiceManagerAPI.Validators;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequestDTO>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate must be after or equal to StartDate");

        RuleFor(x => x.Status)
            .Must(s => new[] {InvoiceStatus.Cancelled,
                              InvoiceStatus.Paid,
                              InvoiceStatus.Sent,
                              InvoiceStatus.Received,
                              InvoiceStatus.Created,
                              InvoiceStatus.Rejected}
            .Contains(s))
            .WithMessage("Status must be one of: 0(Created), 1(Sent), 2(Received), 3(Paid), 4(Canceled), 5(Rejected)");

        RuleFor(x => x.Rows)
            .NotEmpty().WithMessage("Invoice must have at least one row");

        RuleForEach(x => x.Rows).SetValidator(new CreateInvoiceRowValidator());

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment must be less than 500 characters");
    }
}

public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceRequestDTO>
{
    public UpdateInvoiceValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate must be after or equal to StartDate");

        RuleFor(x => x.Status)
            .Must(s => new[] {InvoiceStatus.Cancelled, 
                              InvoiceStatus.Paid, 
                              InvoiceStatus.Sent, 
                              InvoiceStatus.Received, 
                              InvoiceStatus.Created, 
                              InvoiceStatus.Rejected}
            .Contains(s))
            .WithMessage("Status must be one of: 0(Created), 1(Sent), 2(Received), 3(Paid), 4(Canceled), 5(Rejected)");

        RuleFor(x => x.Rows)
            .NotEmpty().WithMessage("Invoice must have at least one row");

        RuleForEach(x => x.Rows).SetValidator(new CreateInvoiceRowValidator());

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment must be less than 500 characters");
    }
}

public class CreateInvoiceRowValidator : AbstractValidator<CreateInvoiceRowRequestDTO>
{
    public CreateInvoiceRowValidator()
    {
        RuleFor(x => x.Service)
            .NotEmpty().WithMessage("Service is required")
            .MaximumLength(100).WithMessage("Service must be less than 100 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");
    }
}

public class InvoiceQueryParamsValidator : AbstractValidator<InvoiceQueryParams>
{
    public InvoiceQueryParamsValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SortDirection)
            .Must(x => string.IsNullOrEmpty(x) || x == "asc" || x == "desc")
            .WithMessage("SortDirection must be either 'asc' or 'desc'");

        RuleFor(x => x.MinTotal)
            .GreaterThanOrEqualTo(0).When(x => x.MinTotal.HasValue)
            .WithMessage("MinTotal must be >= 0");

        RuleFor(x => x.MaxTotal)
            .GreaterThanOrEqualTo(0).When(x => x.MaxTotal.HasValue)
            .WithMessage("MaxTotal must be >= 0");
    }
}