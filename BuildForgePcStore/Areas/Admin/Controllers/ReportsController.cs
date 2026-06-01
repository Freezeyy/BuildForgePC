using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthorize]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SalesReportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SalesReportViewModel model)
    {
        var from = model.DateFrom.Date;
        var to = model.DateTo.Date.AddDays(1);

        var orders = await _context.Orders
            .Where(o => o.OrderDate >= from && o.OrderDate < to)
            .ToListAsync();

        model.OrderCount = orders.Count;
        model.GrandTotal = orders.Sum(o => o.TotalAmount);

        var details = await _context.OrderDetails
            .Include(d => d.Product)
                .ThenInclude(p => p.Category)
            .Include(d => d.Order)
            .Where(d => d.Order.OrderDate >= from && d.Order.OrderDate < to)
            .ToListAsync();

        model.ByCategory = details
            .GroupBy(d => d.Product.Category.CategoryName)
            .Select(g => new SalesReportRowViewModel
            {
                Label = g.Key,
                TotalSales = g.Sum(x => x.Subtotal),
                OrderCount = g.Select(x => x.OrderId).Distinct().Count()
            })
            .OrderByDescending(r => r.TotalSales)
            .ToList();

        model.ByDay = orders
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new SalesReportRowViewModel
            {
                Label = g.Key.ToString("dd MMM yyyy"),
                TotalSales = g.Sum(x => x.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(r => r.Label)
            .ToList();

        return View(model);
    }
}