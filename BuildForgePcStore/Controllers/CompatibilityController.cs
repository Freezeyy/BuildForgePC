using BuildForgePcStore.Data;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Controllers;

public class CompatibilityController : Controller
{
    private readonly ApplicationDbContext _context;

    public CompatibilityController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = await BuildViewModelAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CompatibilityViewModel model)
    {
        await LoadListsAsync(model);

        if (!model.CpuProductId.HasValue || !model.MotherboardProductId.HasValue)
        {
            model.ResultMessage = "Please select both a CPU and a motherboard.";
            return View(model);
        }

        var cpu = await _context.Products.FindAsync(model.CpuProductId);
        var mb = await _context.Products.FindAsync(model.MotherboardProductId);

        if (cpu == null || mb == null || string.IsNullOrEmpty(cpu.Socket) || string.IsNullOrEmpty(mb.Socket))
        {
            model.ResultMessage = "Invalid selection.";
            model.IsCompatible = false;
            return View(model);
        }

        var compatible = string.Equals(cpu.Socket, mb.Socket, StringComparison.OrdinalIgnoreCase);
        model.IsCompatible = compatible;
        model.ResultMessage = compatible
            ? $"Compatible — both use {cpu.Socket}."
            : $"Not compatible — CPU uses {cpu.Socket}, motherboard uses {mb.Socket}.";

        return View(model);
    }

    private async Task<CompatibilityViewModel> BuildViewModelAsync()
    {
        var vm = new CompatibilityViewModel();
        await LoadListsAsync(vm);
        return vm;
    }

    private async Task LoadListsAsync(CompatibilityViewModel vm)
    {
        vm.CpuList = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.CategoryName == "CPU")
            .OrderBy(p => p.ProductName)
            .ToListAsync();

        vm.MotherboardList = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.CategoryName == "Motherboard")
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }
}
