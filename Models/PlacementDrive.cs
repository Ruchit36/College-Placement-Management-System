using System.ComponentModel.DataAnnotations;

namespace CollegePlacementManagementSystem.Models
{
    public class PlacementDrive
    {
        public int Id { get; set; }

    [Required]
        public string? CompanyName { get; set; }

        [Required]
        public string? JobRole { get; set; }

        public double MinimumCGPA { get; set; }

        [DataType(DataType.Date)]
        public DateTime ApplicationDeadline { get; set; }
    }

}
