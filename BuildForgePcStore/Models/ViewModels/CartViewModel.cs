namespace BuildForgePcStore.Models.ViewModels;

public class CartViewModel
{
    public List<CartLineViewModel> Lines { get; set; } = new();
    public decimal Subtotal => Lines.Sum(l => l.LineTotal);
}
public class CartLineViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public int Stock { get; set; }
}