using System.ComponentModel.DataAnnotations;

namespace CollegePlacementManagementSystem.Models
{
    public class PlacementApplication
    {
        public int Id { get; set; }

    [Required]
        public string? StudentName { get; set; }

        [Required]
        public string? CompanyName { get; set; }

        [Required]
        public string? JobRole { get; set; }

        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;

        [Required]
        public string Status { get; set; } = "Pending";
    }

}
