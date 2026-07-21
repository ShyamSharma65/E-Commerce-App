using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Address1 is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage ="Address1 length should be greater than 2")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        public string City { get; set; }

        [Required(ErrorMessage = "Postalcode is required")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

    }
}
