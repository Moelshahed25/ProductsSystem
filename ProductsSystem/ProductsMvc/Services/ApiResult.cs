using System.Text.Json;

namespace ProductsMvc.Services;

public class ApiResult
{
    public bool Success { get; private set; }
    public string? Error { get; private set; }

    public static ApiResult Ok() => new() { Success = true };
    public static ApiResult Fail(string error) => new() { Success = false, Error = error };

    // بيحول رد الـ API لنتيجة، ويطلع رسالة الخطأ لو فيه
    public static async Task<ApiResult> FromResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return Ok();

        var message = $"API returned error {(int)response.StatusCode}.";
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var m) && m.GetString() is { } s)
                message = s;
            else if (doc.RootElement.TryGetProperty("title", out var t) && t.GetString() is { } title)
                message = title;
        }
        catch
        {
            // الرد مش JSON — نكتفي بالرسالة الافتراضية
        }
        return Fail(message);
    }
}
