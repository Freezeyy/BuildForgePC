using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Helpers;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Controllers;

[CustomerAuthorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetUserId()!.Value;
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderSummaryViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderDetails.Sum(d => d.Quantity)
            })
            .ToListAsync();

        return View(new OrderHistoryViewModel { Orders = orders });
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var userId = HttpContext.Session.GetUserId()!.Value;
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

        if (order == null) return NotFound();

        var vm = new OrderDetailsViewModel
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            Subtotal = order.Subtotal,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            Lines = order.OrderDetails.Select(d => new ReceiptLineViewModel
            {
                ProductName = d.Product.ProductName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal
            }).ToList()
        };

        return View(vm);
    }
}