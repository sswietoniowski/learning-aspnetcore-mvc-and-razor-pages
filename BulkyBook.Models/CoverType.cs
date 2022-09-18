using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models;

public class CoverType
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "Cover Type")]
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }
}