﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(ProductId))]
        public string ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage ="Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        [NotMapped]
        public decimal Price { get; set; }
    }
}
