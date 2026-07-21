using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CustomerUpdateDTO
    {
        [Required(ErrorMessage = "CustomerId is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name Length should be greater than 2")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name Length should be greater than 2")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Phone number is invalid")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number is not valid")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "DOB is required")]
        public DateTime DateOfBirth { get; set; }

    }
}
