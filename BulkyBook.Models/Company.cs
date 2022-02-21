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
        public string StreeAddress { get; set; }
        [MaxLength(64)]
        public string City { get; set; }
        [MaxLength(64)]
        public string State { get; set; }
        [MaxLength(16)]
        public string PostalCode { get; set; }
        [MaxLength(16)]
        public string PhoneNumber { get; set; }
        public bool IsAuthorizedCompany { get; set; }
    }
}
