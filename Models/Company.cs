using System.ComponentModel.DataAnnotations;

namespace CollegePlacementManagementSystem.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string? CompanyName { get; set; }

        [Required]
        public string? Industry { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Location { get; set; }
    }
}