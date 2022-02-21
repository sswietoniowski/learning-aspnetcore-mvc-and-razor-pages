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
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(13)]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }
        [Required]
        [MaxLength(128)]
        [Display(Name = "Author")]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        [Column(TypeName="decimal(18,2)")]
        [Display(Name = "List Price")]
        public decimal ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price")]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        [Display(Name = "Price > 50")]
        public decimal Price50 { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        [Display(Name = "Price > 100")]
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
