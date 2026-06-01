using System.ComponentModel.DataAnnotations;

namespace BuildForgePcStore.Models.ViewModels;

public class SalesReportViewModel
{
    [DataType(DataType.Date)]
    [Display(Name = "From")]
    public DateTime DateFrom { get; set; } = DateTime.Today.AddDays(-30);

    [DataType(DataType.Date)]
    [Display(Name = "To")]
    public DateTime DateTo { get; set; } = DateTime.Today;

    public List<SalesReportRowViewModel> ByCategory { get; set; } = new();
    public List<SalesReportRowViewModel> ByDay { get; set; } = new();
    public decimal GrandTotal { get; set; }
    public int OrderCount { get; set; }
}

public class SalesReportRowViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int OrderCount { get; set; }
}