using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CollegePlacementManagementSystem.Models;

namespace CollegePlacementManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Students()
    {
        return View();
    }

    public IActionResult Companies()
    {
        return View();
    }

    public IActionResult Placements()
    {
        return View();
    }

    public IActionResult Applications()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
