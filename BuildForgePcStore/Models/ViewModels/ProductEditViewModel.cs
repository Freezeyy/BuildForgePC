using System.ComponentModel.DataAnnotations;

namespace BuildForgePcStore.Models.ViewModels;

public class ProductEditViewModel
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Product name")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 999999)]
    [Display(Name = "Price (MYR)")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, 100000)]
    public int Stock { get; set; }

    [Display(Name = "Socket (CPU / Motherboard only)")]
    [StringLength(20)]
    public string? Socket { get; set; }

    public string? CurrentImagePath { get; set; }

    [Display(Name = "Product image")]
    public IFormFile? ImageFile { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}
