using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductsMvc.Models;

namespace ProductsMvc.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
