﻿using System.ComponentModel.DataAnnotations;

namespace BookListRazorPagesCRUD.Models;

public class Book
{
    [Key]
    public int Id { get; set; }
    [Required, Display(Name = "Book Name")]
    [MaxLength(128)]
    public string Name { get; set; }
    [Required, Display(Name = "Author Name")]
    [MaxLength(64)]
    public string Author { get; set; }
    public string ISBN { get; set; }
}