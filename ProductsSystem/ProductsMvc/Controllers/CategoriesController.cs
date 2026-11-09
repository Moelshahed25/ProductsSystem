using Microsoft.AspNetCore.Mvc;
using ProductsMvc.Models;
using ProductsMvc.Services;

namespace ProductsMvc.Controllers;

public class CategoriesController : Controller
{
    private const string ApiDown = "Cannot reach the API. Make sure ProductsApi is running.";

    private readonly ICategoryService _categories;
    public CategoriesController(ICategoryService categories) => _categories = categories;

    private static bool IsApiDown(Exception ex)
        => ex is HttpRequestException or TaskCanceledException;

    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _categories.GetAllAsync());
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            ViewBag.Error = ApiDown;
            return View(new List<CategoryViewModel>());
        }
    }

    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var result = await _categories.CreateAsync(model);
            if (result.Success)
            {
                TempData["Success"] = "Category added successfully.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, result.Error!);
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            ModelState.AddModelError(string.Empty, ApiDown);
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var category = await _categories.GetByIdAsync(id);
            if (category is null) return NotFound();
            return View(new CategoryFormViewModel { Id = category.Id, Name = category.Name });
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            TempData["Error"] = ApiDown;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        model.Id = id;
        if (!ModelState.IsValid) return View(model);
        try
        {
            var result = await _categories.UpdateAsync(id, model);
            if (result.Success)
            {
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, result.Error!);
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            ModelState.AddModelError(string.Empty, ApiDown);
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _categories.DeleteAsync(id);
            if (result.Success) TempData["Success"] = "Category deleted.";
            else TempData["Error"] = result.Error;
        }
        catch (Exception ex) when (IsApiDown(ex))
        {
            TempData["Error"] = ApiDown;
        }
        return RedirectToAction(nameof(Index));
    }
}
