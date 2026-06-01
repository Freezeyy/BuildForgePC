using System.ComponentModel.DataAnnotations;
namespace BuildForgePcStore.Models.ViewModels;

public class CheckoutViewModel
{
    public CartViewModel Cart { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm your order.")]
    [Display(Name = "I confirm this order")]
    public bool ConfirmOrder { get; set; }
}