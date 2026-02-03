using AutoMapper;
using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;

namespace InvoiceManagerAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer Mappings
        CreateMap<Customer, CustomerResponseDTO>();

        CreateMap<CreateCustomerRequestDTO, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        CreateMap<UpdateCustomerRequestDTO, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        // Invoice Mappings
        CreateMap<Invoice, InvoiceResponseDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer!.Name));

        CreateMap<CreateInvoiceRequestDTO, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalSum, opt => opt.MapFrom(
                src => src.Rows != null ? src.Rows.Sum(r => r.Quantity * r.Amount) : 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Rows, opt => opt.MapFrom(src => src.Rows))
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        CreateMap<UpdateInvoiceRequestDTO, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalSum, opt => opt.MapFrom(
                src => src.Rows != null ? src.Rows.Sum(r => r.Quantity * r.Amount) : 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Rows, opt => opt.MapFrom(src => src.Rows))
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        // InvoiceRow Mappings
        CreateMap<CreateInvoiceRowRequestDTO, InvoiceRow>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
            .ForMember(dest => dest.Sum, opt => opt.MapFrom(src => src.Quantity * src.Amount));
    }
}
