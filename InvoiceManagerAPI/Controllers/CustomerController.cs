using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseDTO>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponseDTO>> GetById(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer is null)
        {
            return NotFound($"Customer with ID {id} not found");
        }
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDTO>> Create([FromBody]CreateCustomerRequestDTO customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var createdCustomer = await _customerService.CreateAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerResponseDTO>> Update(Guid id, [FromBody]UpdateCustomerRequestDTO customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var updatedCustomer = await _customerService.UpdateAsync(id, customer);
        if (updatedCustomer is null)
        {
            return NotFound($"Customer with ID {id} not found");
        }
        return Ok(updatedCustomer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deletedCustomer = await _customerService.DeleteAsync(id);
        if (!deletedCustomer)
        {
            return BadRequest("Cannot delete customer: either customer does not exist or has invoices.");
        }
        return NoContent();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var archivedCustomer = await _customerService.ArchiveAsync(id);
        if (!archivedCustomer)
        {
            return NotFound($"Customer with ID {id} not found");
        }
        return NoContent();
    }
}
