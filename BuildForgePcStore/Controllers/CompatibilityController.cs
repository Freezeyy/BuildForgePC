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

        var cpu = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p =>
                p.ProductId == model.CpuProductId.Value &&
                p.Category.CategoryName == "CPU");

        var mb = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p =>
                p.ProductId == model.MotherboardProductId.Value &&
                p.Category.CategoryName == "Motherboard");

        if (cpu == null || mb == null)
        {
            model.ResultMessage = "Invalid selection — pick one CPU and one motherboard from the lists.";
            model.IsCompatible = null;
            return View(model);
        }

        var cpuSocket = cpu.Socket?.Trim();
        var mbSocket = mb.Socket?.Trim();

        if (string.IsNullOrEmpty(cpuSocket) || string.IsNullOrEmpty(mbSocket))
        {
            model.ResultMessage = "Socket information is missing for the selected products.";
            model.IsCompatible = null;
            return View(model);
        }

        var compatible = string.Equals(cpuSocket, mbSocket, StringComparison.OrdinalIgnoreCase);
        model.IsCompatible = compatible;
        model.ResultMessage = compatible
            ? $"Compatible — both use {cpuSocket}."
            : $"Not compatible — CPU uses {cpuSocket}, motherboard uses {mbSocket}.";

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
