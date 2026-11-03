using Microsoft.AspNetCore.Authorization;
using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegePlacementManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            string enteredName = student.FullName?.Trim() ?? string.Empty;
            string enteredCourse = student.Course?.Trim() ?? string.Empty;

            bool studentExists = await _context.Students.AnyAsync(s =>
                s.FullName == enteredName &&
                s.Course == enteredCourse
            );

            if (studentExists)
            {
                ModelState.AddModelError(
                    "FullName",
                    "A student with this name and course is already registered."
                );
            }
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            string enteredName = student.FullName?.Trim() ?? string.Empty;
            string enteredCourse = student.Course?.Trim() ?? string.Empty;

            bool duplicateStudent = await _context.Students.AnyAsync(s =>
                s.FullName == enteredName &&
                s.Course == enteredCourse &&
                s.Id != student.Id
            );

            if (duplicateStudent)
            {
                ModelState.AddModelError(
                    "FullName",
                    "A student with this name and course is already registered."
                );
            }

            if (ModelState.IsValid)
            {
                _context.Students.Update(student);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.ToListAsync();
            return View(students);
        }
    }
}