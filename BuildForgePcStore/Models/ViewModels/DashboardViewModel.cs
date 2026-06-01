namespace BuildForgePcStore.Models.ViewModels;

public class DashboardViewModel
{
    public decimal TotalSales { get; set; }
    public decimal MonthlySales { get; set; }
    public int OrderCount { get; set; }
    public int ProductCount { get; set; }
    public int LowStockCount { get; set; }
    public List<TopProductViewModel> TopProducts { get; set; } = new();
}

public class TopProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}