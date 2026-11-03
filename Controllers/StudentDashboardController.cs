using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CollegePlacementManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentDashboardController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Student Dashboard
        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return Forbid();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s =>
                    s.Email != null &&
                    s.Email.ToLower() == email.ToLower());

            if (student == null)
            {
                return NotFound("Student record not found.");
            }

            var placementDrives = await _context.PlacementDrives
                .Where(d => d.MinimumCGPA <= student.CGPA &&
                            d.ApplicationDeadline >= DateTime.Today)
                .ToListAsync();

            var myApplications = await _context.PlacementApplications
                .Where(a => a.StudentName == student.FullName)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            ViewBag.PlacementDrives = placementDrives;
            ViewBag.MyApplications = myApplications;

            return View(student);
        }

        // Apply for Placement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return Challenge();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s =>
                    s.Email != null &&
                    s.Email.ToLower() == user.Email.ToLower());

            if (student == null)
            {
                return NotFound("Student record not found.");
            }

            var drive = await _context.PlacementDrives
                .FindAsync(id);

            if (drive == null)
            {
                return NotFound("Placement drive not found.");
            }

            // Check application deadline
            if (drive.ApplicationDeadline.Date < DateTime.Today)
            {
                TempData["ErrorMessage"] =
                    "Application deadline has passed.";

                return RedirectToAction("Index");
            }

            // Check CGPA eligibility
            if (student.CGPA < drive.MinimumCGPA)
            {
                TempData["ErrorMessage"] =
                    "Your CGPA does not meet the eligibility criteria.";

                return RedirectToAction("Index");
            }

            // Check duplicate application
            var alreadyApplied = await _context.PlacementApplications
                .AnyAsync(a =>
                    a.StudentName == student.FullName &&
                    a.CompanyName == drive.CompanyName &&
                    a.JobRole == drive.JobRole);

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] =
                    "You have already applied for this position.";

                return RedirectToAction("Index");
            }

            // Create new application
            var application = new PlacementApplication
            {
                StudentName = student.FullName,
                CompanyName = drive.CompanyName,
                JobRole = drive.JobRole,
                ApplicationDate = DateTime.Today,
                Status = "Pending"
            };

            _context.PlacementApplications.Add(application);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your application has been submitted successfully!";

            return RedirectToAction("Index");
        }
    }
}