using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Models
{
    [Index(nameof(Email), Name = "IX_EMAIL_UNIQUE", IsUnique = true)]
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name length should me greater 2")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name length should me greater 2")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public string Email{ get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "DOB is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Feedback> FeedBacks { get; set; }
    }
}
