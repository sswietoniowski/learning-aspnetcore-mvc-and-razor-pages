using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(64)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [MaxLength(128)]
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
        [NotMapped]
        [MaxLength(64)]
        [Display(Name = "Role")]
        public string Role { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
