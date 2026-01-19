using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TokoApi.Models;

public class TransactionItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    [Required]
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Qty { get; set; }

    [Column(TypeName = "numeric(18,2)")]
    public decimal PriceSnapshot { get; set; }
}
