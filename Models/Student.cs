using System.ComponentModel.DataAnnotations;

namespace CollegePlacementManagementSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Course { get; set; }

        public double CGPA { get; set; }

        public int GraduationYear { get; set; }
    }
}