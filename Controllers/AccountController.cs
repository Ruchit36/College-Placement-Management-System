using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegePlacementManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Login Portal Selection
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Admin Login
        [HttpGet]
        public IActionResult AdminLogin(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(
            AdminLoginViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(
                model.Username ?? string.Empty);

            if (user == null ||
                !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid admin username or password.");

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password ?? string.Empty,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(
                string.Empty,
                "Invalid admin username or password.");

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // Student Login
        [HttpGet]
        public IActionResult StudentLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentLogin(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(
                model.Username?.Trim() ?? string.Empty);

            if (user == null ||
                !await _userManager.IsInRoleAsync(user, "Student"))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid student email or password.");

                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password ?? string.Empty,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "StudentDashboard");
            }

            ModelState.AddModelError(
                string.Empty,
                "Invalid student email or password.");

            return View(model);
        }

        // Student Registration
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email!.Trim();

            var student = await _context.Students.FirstOrDefaultAsync(s =>
                s.Id == model.StudentId &&
                s.Email != null &&
                s.Email.ToLower() == email.ToLower());

            if (student == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Student ID or registered email does not match our records.");

                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(
                model.Email!.Trim());

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An account with this email already exists.");

                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Student"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Student"));
            }

            var newUser = new IdentityUser
            {
                UserName = model.Email.Trim(),
                Email = model.Email.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(
                newUser,
                model.Password!);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(
                    newUser, "Student");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);

                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty, error.Description);
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Registration successful! You can now log in.";

                return RedirectToAction("StudentLogin");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }

}
