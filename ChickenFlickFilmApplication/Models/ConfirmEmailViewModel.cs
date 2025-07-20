using System.ComponentModel.DataAnnotations;

namespace ChickenFlickFilmApplication.Models
{
    public class ConfirmEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public required string Code { get; set; }
        public string CodeEnter { get; set; }
    }
}
