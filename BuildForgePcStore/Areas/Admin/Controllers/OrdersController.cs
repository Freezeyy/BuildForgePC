using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var query = _context.Orders.Include(o => o.User).AsQueryable();

        if (from.HasValue)
            query = query.Where(o => o.OrderDate >= from.Value.Date);
        if (to.HasValue)
            query = query.Where(o => o.OrderDate < to.Value.Date.AddDays(1));

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new AdminOrderRowViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerName = o.User.Name,
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderDetails.Sum(d => d.Quantity)
            })
            .ToListAsync();

        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");

        return View(orders);
    }
}