using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PCShop.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ValidateNever]
        public IdentityUser User { get; set; } = null!;

        [Required(ErrorMessage = "Numele de contact este obligatoriu.")]
        [StringLength(100)]
        [Display(Name = "Nume de contact")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Email invalid.")]
        [Display(Name = "Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Numar de telefon invalid.")]
        [Display(Name = "Telefon")]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Descrierea problemei este obligatorie.")]
        [StringLength(1000)]
        [Display(Name = "Descrierea problemei")]
        public string ProblemDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data predarii este obligatorie.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data predarii")]
        public DateTime DropOffDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Data crearii")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}