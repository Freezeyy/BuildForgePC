using System.ComponentModel.DataAnnotations;

namespace BuildForgePcStore.Models.ViewModels;

public class CategoryEditViewModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(50)]
    [Display(Name = "Category name")]
    public string CategoryName { get; set; } = string.Empty;
}
