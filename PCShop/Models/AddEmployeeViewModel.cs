using System.ComponentModel.DataAnnotations;

namespace PCShop.Models
{
    public class AddEmployeeViewModel
    {
        [Required(ErrorMessage = "The email is mandatory.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The password is mandatory.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "You need to select a role.")]
        public string Role { get; set; } = string.Empty; 
    }
}