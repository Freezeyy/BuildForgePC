using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var vm = new DashboardViewModel
        {
            TotalSales = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
            MonthlySales = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
            OrderCount = await _context.Orders.CountAsync(),
            ProductCount = await _context.Products.CountAsync(),
            LowStockCount = await _context.Products.CountAsync(p => p.Stock < 5)
        };

        var top = await _context.OrderDetails
            .GroupBy(d => d.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Qty = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Subtotal)
            })
            .OrderByDescending(x => x.Qty)
            .Take(5)
            .ToListAsync();

        var productIds = top.Select(t => t.ProductId).ToList();
        var names = await _context.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId, p => p.ProductName);

        vm.TopProducts = top.Select(t => new TopProductViewModel
        {
            ProductName = names.GetValueOrDefault(t.ProductId, "Unknown"),
            QuantitySold = t.Qty,
            Revenue = t.Revenue
        }).ToList();

        return View(vm);
    }
}