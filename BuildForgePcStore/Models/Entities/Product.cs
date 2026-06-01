using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildForgePcStore.Models.Entities;

public class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    [Required, MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    [Required, MaxLength(300)]
    public string ImagePath { get; set; } = "/images/products/placeholder.svg";

    /// <summary>CPU and Motherboard only — e.g. AM4, AM5, LGA1700</summary>
    [MaxLength(20)]
    public string? Socket { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
