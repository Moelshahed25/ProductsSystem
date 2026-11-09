using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Dtos;
using ProductsApi.Models;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context) => _context = context;

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var list = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, ProductsCount = c.Products.Count })
            .ToListAsync();
        return list;
    }

    // GET: api/categories/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var dto = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, ProductsCount = c.Products.Count })
            .FirstOrDefaultAsync();
        return dto is null ? NotFound() : dto;
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategorySaveDto input)
    {
        var category = new Category { Name = input.Name.Trim() };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var dto = new CategoryDto { Id = category.Id, Name = category.Name, ProductsCount = 0 };
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, dto);
    }

    // PUT: api/categories/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategorySaveDto input)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return NotFound();

        category.Name = input.Name.Trim();
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/categories/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return NotFound();

        if (await _context.Products.AnyAsync(p => p.CategoryId == id))
            return Conflict(new { message = "Cannot delete a category that still has products." });

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
