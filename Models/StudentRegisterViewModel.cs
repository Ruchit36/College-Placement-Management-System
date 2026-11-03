using System.ComponentModel.DataAnnotations;

namespace CollegePlacementManagementSystem.Models
{
    public class StudentRegisterViewModel
    {
        [Required(ErrorMessage = "Student ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid Student ID.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must contain at least 8 characters.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Password and Confirm Password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

}
