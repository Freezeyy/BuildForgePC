using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
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
        ViewBag.ProductCount = await _context.Products.CountAsync();
        ViewBag.CategoryCount = await _context.Categories.CountAsync();
        ViewBag.UserCount = await _context.Users.CountAsync();
        ViewBag.OrderCount = await _context.Orders.CountAsync();
        return View();
    }
}
