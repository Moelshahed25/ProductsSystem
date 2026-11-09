using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductsMvc.Models;
using ProductsMvc.Services;

namespace ProductsMvc.Controllers;

public class ProductsController : Controller
{
    private const string ApiDown = "Cannot reach the API. Make sure ProductsApi is running.";

    private readonly IProductService _products;
    private readonly ICategoryService _categories;

    public ProductsController(IProductService products, ICategoryService categories)
    {
        _products = products;
        _categories = categories;
    }

    private static bool IsApiDown(Exception ex)
        => ex is HttpRequestException or TaskCanceledException;

    private async Task FillCategoriesAsync(ProductFormViewModel model)
    {
        var cats = await _categories.GetAllAsync();
        model.Categories = new SelectList(cats, "Id", "Name", model.CategoryId);
    }

    // GET: /Products  (Add button + list with Edit/Delete)
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _products.GetAllAsync());
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            ViewBag.Error = ApiDown;
            return View(new List<ProductViewModel>());
        }
    }

    // GET: /Products/Create
    public async Task<IActionResult> Create()
    {
        var model = new ProductFormViewModel();
        try { await FillCategoriesAsync(model); }
        catch (Exception ex) when (IsApiDown(ex))
        {
            TempData["Error"] = ApiDown;
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // POST: /Products/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _products.CreateAsync(model);
                if (result.Success)
                {
                    TempData["Success"] = "Product added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Error!);
            }
            await FillCategoriesAsync(model);
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            ModelState.AddModelError(string.Empty, ApiDown);
            model.Categories = new List<SelectListItem>();
        }
        return View(model);
    }

    // GET: /Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var product = await _products.GetByIdAsync(id);
            if (product is null) return NotFound();

            var model = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            await FillCategoriesAsync(model);
            return View(model);
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            TempData["Error"] = ApiDown;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Products/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        model.Id = id;
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _products.UpdateAsync(id, model);
                if (result.Success)
                {
                    TempData["Success"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Error!);
            }
            await FillCategoriesAsync(model);
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            ModelState.AddModelError(string.Empty, ApiDown);
            model.Categories = new List<SelectListItem>();
        }
        return View(model);
    }

    // POST: /Products/Delete/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _products.DeleteAsync(id);
            if (result.Success) TempData["Success"] = "Product deleted.";
            else TempData["Error"] = result.Error;
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            TempData["Error"] = ApiDown;
        }
        return RedirectToAction(nameof(Index));
    }
}
