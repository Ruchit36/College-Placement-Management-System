using Microsoft.AspNetCore.Authorization;
using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegePlacementManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            string enteredName = company.CompanyName?.Trim() ?? string.Empty;
            string enteredIndustry = company.Industry?.Trim() ?? string.Empty;
            string enteredLocation = company.Location?.Trim() ?? string.Empty;

            bool companyExists = await _context.Companies.AnyAsync(c =>
                c.CompanyName == enteredName &&
                c.Industry == enteredIndustry &&
                c.Location == enteredLocation
            );

            if (companyExists)
            {
                ModelState.AddModelError(
                    "CompanyName",
                    "This company with the same industry and location is already registered."
                );
            }
            if (ModelState.IsValid)
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            string enteredName = company.CompanyName?.Trim() ?? string.Empty;
            string enteredIndustry = company.Industry?.Trim() ?? string.Empty;
            string enteredLocation = company.Location?.Trim() ?? string.Empty;

            bool duplicateCompany = await _context.Companies.AnyAsync(c =>
                c.CompanyName == enteredName &&
                c.Industry == enteredIndustry &&
                c.Location == enteredLocation &&
                c.Id != company.Id
            );

            if (duplicateCompany)
            {
                ModelState.AddModelError(
                    "CompanyName",
                    "This company with the same industry and location is already registered."
                );
            }

            if (ModelState.IsValid)
            {
                _context.Companies.Update(company);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}