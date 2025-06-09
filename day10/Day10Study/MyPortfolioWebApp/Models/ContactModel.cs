using System.ComponentModel.DataAnnotations;

namespace MyPortfolioWebApp.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "필수입니다")]
        public string Name { get; set; }

        [Required(ErrorMessage = "필수입니다")]
        public string Email { get; set; }

        [Required(ErrorMessage = "필수입니다")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "필수입니다")]
        public string Message { get; set; }
    }
}
