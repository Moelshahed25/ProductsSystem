using System.Net;
using ProductsMvc.Models;

namespace ProductsMvc.Services;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> GetAllAsync();
    Task<CategoryViewModel?> GetByIdAsync(int id);
    Task<ApiResult> CreateAsync(CategoryFormViewModel model);
    Task<ApiResult> UpdateAsync(int id, CategoryFormViewModel model);
    Task<ApiResult> DeleteAsync(int id);
}

public class CategoryService : ICategoryService
{
    private readonly HttpClient _http;
    public CategoryService(HttpClient http) => _http = http;

    public async Task<List<CategoryViewModel>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<CategoryViewModel>>("api/categories") ?? new();

    public async Task<CategoryViewModel?> GetByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/categories/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryViewModel>();
    }

    public async Task<ApiResult> CreateAsync(CategoryFormViewModel model)
        => await ApiResult.FromResponseAsync(
            await _http.PostAsJsonAsync("api/categories", new { name = model.Name }));

    public async Task<ApiResult> UpdateAsync(int id, CategoryFormViewModel model)
        => await ApiResult.FromResponseAsync(
            await _http.PutAsJsonAsync($"api/categories/{id}", new { name = model.Name }));

    public async Task<ApiResult> DeleteAsync(int id)
        => await ApiResult.FromResponseAsync(await _http.DeleteAsync($"api/categories/{id}"));
}
