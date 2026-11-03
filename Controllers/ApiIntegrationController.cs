using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CollegePlacementManagementSystem.Controllers
{
    [Authorize]
    public class ApiIntegrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
