using System.ComponentModel.DataAnnotations;

namespace ChickenFlickFilmApplication.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
