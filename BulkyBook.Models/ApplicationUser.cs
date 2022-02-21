using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string StreeAddress { get; set; }
        [MaxLength(64)]
        public string City { get; set; }
        [MaxLength(64)]
        public string State { get; set; }
        [MaxLength(16)]
        public string PostalCode { get; set; }
        [NotMapped]
        [MaxLength(64)]
        public string Role { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
