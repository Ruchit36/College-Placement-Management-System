using Microsoft.AspNetCore.Authorization;
using CollegePlacementManagementSystem.Data;
using CollegePlacementManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CollegePlacementManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/StudentsApi
        [HttpGet]
        [Authorize(Roles = "Admin,Student")]

        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();

            return Ok(students);
        }

        // GET: api/StudentsApi/1
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Student")]

        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return Ok(student);
        }

        // POST: api/StudentsApi
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateStudent(Student student)
        {

            string enteredName = student.FullName?.Trim() ?? string.Empty;
            string enteredCourse = student.Course?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(enteredName) ||
                string.IsNullOrWhiteSpace(enteredCourse))
            {
                return BadRequest(new
                {
                    message = "Student name and course are required."
                });
            }

            bool duplicate = await _context.Students.AnyAsync(s =>
                s.FullName == enteredName &&
                s.Course == enteredCourse
            );

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "A student with this name and course already exists."
                });
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = student.Id },
                student
            );
        }

        // PUT: api/StudentsApi/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest(new
                {
                    message = "Student ID mismatch."
                });
            }

            bool studentExists = await _context.Students
                .AnyAsync(s => s.Id == id);

            if (!studentExists)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            string enteredName = student.FullName?.Trim() ?? string.Empty;
            string enteredCourse = student.Course?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(enteredName) ||
                string.IsNullOrWhiteSpace(enteredCourse))
            {
                return BadRequest(new
                {
                    message = "Student name and course are required."
                });
            }

            bool duplicate = await _context.Students.AnyAsync(s =>
                s.FullName == enteredName &&
                s.Course == enteredCourse &&
                s.Id != id
            );

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "A student with this name and course already exists."
                });
            }

            _context.Entry(student).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Student updated successfully.",
                student
            });
        }

        // DELETE: api/StudentsApi/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Student deleted successfully."
            });
        }

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Protected()
        {
            return Ok(new
            {
                message = "Authentication successful! You have accessed a protected API.",
                username = User.Identity?.Name
            });
        }
    }
}