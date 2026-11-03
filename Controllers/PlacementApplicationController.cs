using Microsoft.AspNetCore.Authorization;
using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegePlacementManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlacementApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applications = await _context.PlacementApplications.ToListAsync();
            return View(applications);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlacementApplication application)
        {
            string enteredStudent = application.StudentName?.Trim() ?? string.Empty;
            string enteredCompany = application.CompanyName?.Trim() ?? string.Empty;
            string enteredRole = application.JobRole?.Trim() ?? string.Empty;

            bool duplicateApplication = await _context.PlacementApplications.AnyAsync(a =>
                a.StudentName == enteredStudent &&
                a.CompanyName == enteredCompany &&
                a.JobRole == enteredRole
            );

            if (duplicateApplication)
            {
                ModelState.AddModelError(
                    "StudentName",
                    "This student has already applied for this job role."
                );
            }

            if (ModelState.IsValid)
            {
                _context.PlacementApplications.Add(application);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.PlacementApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlacementApplication application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            string enteredStudent = application.StudentName?.Trim() ?? string.Empty;
            string enteredCompany = application.CompanyName?.Trim() ?? string.Empty;
            string enteredRole = application.JobRole?.Trim() ?? string.Empty;

            bool duplicateApplication = await _context.PlacementApplications.AnyAsync(a =>
                a.StudentName == enteredStudent &&
                a.CompanyName == enteredCompany &&
                a.JobRole == enteredRole &&
                a.Id != application.Id
            );

            if (duplicateApplication)
            {
                ModelState.AddModelError(
                    "StudentName",
                    "This student has already applied for this job role."
                );
            }

            if (ModelState.IsValid)
            {
                _context.PlacementApplications.Update(application);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(application);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.PlacementApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.PlacementApplications.FindAsync(id);

            if (application != null)
            {
                _context.PlacementApplications.Remove(application);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }

    }

}
