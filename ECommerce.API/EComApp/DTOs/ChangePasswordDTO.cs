using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage ="customerid is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "CurrentPassword is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
    }
}
