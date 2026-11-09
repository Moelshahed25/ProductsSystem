using System.Net;
using ProductsMvc.Models;

namespace ProductsMvc.Services;

public interface IProductService
{
    Task<List<ProductViewModel>> GetAllAsync();
    Task<ProductViewModel?> GetByIdAsync(int id);
    Task<ApiResult> CreateAsync(ProductFormViewModel model);
    Task<ApiResult> UpdateAsync(int id, ProductFormViewModel model);
    Task<ApiResult> DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly HttpClient _http;
    public ProductService(HttpClient http) => _http = http;

    public async Task<List<ProductViewModel>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<ProductViewModel>>("api/products") ?? new();

    public async Task<ProductViewModel?> GetByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/products/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductViewModel>();
    }

    private static object ToPayload(ProductFormViewModel m) => new
    {
        name = m.Name,
        price = m.Price,
        categoryId = m.CategoryId
    };

    public async Task<ApiResult> CreateAsync(ProductFormViewModel model)
        => await ApiResult.FromResponseAsync(
            await _http.PostAsJsonAsync("api/products", ToPayload(model)));

    public async Task<ApiResult> UpdateAsync(int id, ProductFormViewModel model)
        => await ApiResult.FromResponseAsync(
            await _http.PutAsJsonAsync($"api/products/{id}", ToPayload(model)));

    public async Task<ApiResult> DeleteAsync(int id)
        => await ApiResult.FromResponseAsync(await _http.DeleteAsync($"api/products/{id}"));
}
