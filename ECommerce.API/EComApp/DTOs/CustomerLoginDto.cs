using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CustomerLoginDto
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(5,ErrorMessage ="Invalid password")]
        public string Password { get; set; }
    }
}
