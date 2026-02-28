using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<InvoiceResponseDTO>>> GetAll()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(invoices);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceResponseDTO>>> GetPaged([FromQuery] InvoiceQueryParams queryParams)
    {
        var invoices = await _invoiceService.GetPagedAsync(queryParams);
        return Ok(invoices);
    }

    [HttpGet("id")]
    public async Task<ActionResult<InvoiceResponseDTO>> GetById(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice is null)
        {
            return NotFound($"Invoice with ID {id} not found");
        }
        return Ok(invoice);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDTO>> Create([FromBody] CreateInvoiceRequestDTO invoice)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var createdInvoice = await _invoiceService.CreateAsync(invoice);
        return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, createdInvoice);
    }

    [HttpPut("id")]
    public async Task<ActionResult<InvoiceResponseDTO>> Update(Guid id, [FromBody] UpdateInvoiceRequestDTO invoice)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var updatedInvoice = await _invoiceService.UpdateAsync(id, invoice);
        if (updatedInvoice is null)
        {
            return NotFound($"Invoice with ID {id} not found");
        }
        return Ok(updatedInvoice);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _invoiceService.DeleteAsync(id);
        if (!result)
        {
            return NotFound($"Invoice with ID {id} not found or cannot be deleted");
        }
        return NoContent();
    }

    [HttpPost("id/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var result = await _invoiceService.ArchiveAsync(id);
        if (!result)
        {
            return NotFound($"Invoice with ID {id} not found");
        }
        return NoContent();
    }

    [HttpPost("id/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string newStatus)
    {
        try
        {
            await _invoiceService.ChangeStatusAsync(id, newStatus);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("id/pdf")]
    public async Task<IActionResult> GeneratePdf(Guid id)
    {
        try
        {
            var pdfData = await _invoiceService.GeneratePdfAsync(id);
            return File(pdfData, "application/pdf", $"Invoice_{id}.pdf");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
