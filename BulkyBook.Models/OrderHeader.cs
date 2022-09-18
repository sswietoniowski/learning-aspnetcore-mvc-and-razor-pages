﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models;

public class OrderHeader
{
    [Key]
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }
    [Required]
    public DateTime ShippingDate { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal OrderTotal { get; set; }
    public string TrackingNumber { get; set; }
    public string Carrier { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime PaymentDueDate { get; set; }
    public string TransactionId { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string StreetAddress { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string State { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string Name { get; set; }
}