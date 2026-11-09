using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Dtos;
using ProductsApi.Models;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context) => _context = context;

    private IQueryable<ProductDto> Query() =>
        _context.Products
            .AsNoTracking()
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name
            });

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        => await Query().OrderBy(p => p.Id).ToListAsync();

    // GET: api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var dto = await Query().FirstOrDefaultAsync(p => p.Id == id);
        return dto is null ? NotFound() : dto;
    }

    // POST: api/products   (Add)
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductSaveDto input)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == input.CategoryId))
            return BadRequest(new { message = "Category does not exist." });

        var product = new Product
        {
            Name = input.Name.Trim(),
            Price = input.Price,
            CategoryId = input.CategoryId
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var dto = await Query().FirstAsync(p => p.Id == product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, dto);
    }

    // PUT: api/products/5   (Edit)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductSaveDto input)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        if (!await _context.Categories.AnyAsync(c => c.Id == input.CategoryId))
            return BadRequest(new { message = "Category does not exist." });

        product.Name = input.Name.Trim();
        product.Price = input.Price;
        product.CategoryId = input.CategoryId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/products/5   (Delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
