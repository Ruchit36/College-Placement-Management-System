using Microsoft.AspNetCore.Authorization;
using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegePlacementManagementSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class PlacementDriveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementDriveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var drives = await _context.PlacementDrives.ToListAsync();
            return View(drives);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlacementDrive drive)
        {
            string enteredCompany = drive.CompanyName?.Trim() ?? string.Empty;
            string enteredRole = drive.JobRole?.Trim() ?? string.Empty;

            bool duplicateDrive = await _context.PlacementDrives.AnyAsync(d =>
                d.CompanyName == enteredCompany &&
                d.JobRole == enteredRole
            );

            if (duplicateDrive)
            {
                ModelState.AddModelError(
                    "JobRole",
                    "This placement drive already exists for this company."
                );
            }

            if (ModelState.IsValid)
            {
                _context.PlacementDrives.Add(drive);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(drive);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drive = await _context.PlacementDrives.FindAsync(id);

            if (drive == null)
            {
                return NotFound();
            }

            return View(drive);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlacementDrive drive)
        {
            if (id != drive.Id)
            {
                return NotFound();
            }

            string enteredCompany = drive.CompanyName?.Trim() ?? string.Empty;
            string enteredRole = drive.JobRole?.Trim() ?? string.Empty;

            bool duplicateDrive = await _context.PlacementDrives.AnyAsync(d =>
                d.CompanyName == enteredCompany &&
                d.JobRole == enteredRole &&
                d.Id != drive.Id
            );

            if (duplicateDrive)
            {
                ModelState.AddModelError(
                    "JobRole",
                    "This placement drive already exists for this company."
                );
            }

            if (ModelState.IsValid)
            {
                _context.PlacementDrives.Update(drive);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(drive);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drive = await _context.PlacementDrives.FindAsync(id);

            if (drive == null)
            {
                return NotFound();
            }

            return View(drive);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drive = await _context.PlacementDrives.FindAsync(id);


            if (drive != null)
            {
                _context.PlacementDrives.Remove(drive);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }

    }

}
