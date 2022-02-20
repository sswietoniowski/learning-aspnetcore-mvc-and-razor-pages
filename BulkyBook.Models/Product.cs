using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(13)]
        public string ISBN { get; set; }
        [Required]
        [MaxLength(128)]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        [Column(TypeName="decimal(18,2)")]
        public decimal ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, 10000)]
        public decimal Price50 { get; set; }
        [Required]
        [Range(1, 10000)]
        public decimal Price100 { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Required]
        public int CoverTypeId { get; set; }
        [ForeignKey(nameof(CoverTypeId))]
        public CoverType CoverType { get; set; }
    }
}
