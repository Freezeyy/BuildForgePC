using System.ComponentModel.DataAnnotations;

namespace BuildForgePcStore.Models.Entities;

public class Category
{
    public int CategoryId { get; set; }

    [Required, MaxLength(50)]
    public string CategoryName { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
