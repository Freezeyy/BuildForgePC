namespace BuildForgePcStore.Models.ViewModels;

public class OrderHistoryViewModel
{
    public List<OrderSummaryViewModel> Orders { get; set; } = new();
}

public class OrderSummaryViewModel
{
    public int OrderId { get; set; }
    /// <summary>1-based sequence for this customer (first order = 1).</summary>
    public int CustomerOrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public class OrderDetailsViewModel
{
    public int OrderId { get; set; }
    public int CustomerOrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ReceiptLineViewModel> Lines { get; set; } = new();
}