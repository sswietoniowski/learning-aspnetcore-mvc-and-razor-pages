using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
        [Display(Name = "Street")]
        public string StreetAddress { get; set; }
        [MaxLength(64)]
        [Display(Name = "City")]
        public string City { get; set; }
        [MaxLength(64)]
        [Display(Name = "State")]
        public string State { get; set; }
        [MaxLength(16)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [MaxLength(16)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Is Authorized")]
        public bool IsAuthorizedCompany { get; set; }
    }
}
