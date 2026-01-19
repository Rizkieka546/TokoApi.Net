using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TokoApi.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [Column(TypeName = "numeric(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string ImageUrl { get; set; } = "";
    public string Description { get; set; } = "";

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
}
