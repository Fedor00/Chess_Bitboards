using System.ComponentModel.DataAnnotations;

namespace UserMicroservice.DTOs
{
    public class RegisterDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}