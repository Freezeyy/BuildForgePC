using BuildForgePcStore.Models.Entities;

namespace BuildForgePcStore.Models.ViewModels;

public class CompatibilityViewModel
{
    public int? CpuProductId { get; set; }
    public int? MotherboardProductId { get; set; }
    public List<Product> CpuList { get; set; } = new();
    public List<Product> MotherboardList { get; set; } = new();
    public bool? IsCompatible { get; set; }
    public string? ResultMessage { get; set; }
}
