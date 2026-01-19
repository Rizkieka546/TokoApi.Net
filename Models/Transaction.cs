using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TokoApi.Models;

public class Transaction
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "numeric(18,2)")]
    public decimal TotalPrice { get; set; }

    [Required, MaxLength(20)]
    public string PaymentMethod { get; set; } = null!;

    [Column(TypeName = "numeric(18,2)")]
    public decimal PaidAmount { get; set; }

    [Column(TypeName = "numeric(18,2)")]
    public decimal ChangeAmount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid KasirId { get; set; }
    public User Kasir { get; set; } = null!;

    public ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();
}
