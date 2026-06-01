using BuildForgePcStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BuildForgePcStore.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(int? categoryId)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        ViewBag.Categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        ViewBag.SelectedCategoryId = categoryId;
        var products = await query.OrderBy(p => p.ProductName).ToListAsync();
        return View(products);
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var product = await _context.Products
        .Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound();
        return View(product);
    }
}