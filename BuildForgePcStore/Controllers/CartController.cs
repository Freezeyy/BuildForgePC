using BuildForgePcStore.Data;
using BuildForgePcStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BuildForgePcStore.Controllers;

public class CartController : Controller
{
    private readonly CartService _cart;
    private readonly ApplicationDbContext _context;
    public CartController(CartService cart, ApplicationDbContext context)
    {
        _cart = cart;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var model = await _cart.BuildCartViewModelAsync();
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();
        if (quantity < 1) quantity = 1;
        _cart.AddItem(productId, quantity);
        TempData["Success"] = $"{product.ProductName} added to cart.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity);
        TempData["Success"] = "Cart updated.";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        _cart.RemoveItem(productId);
        TempData["Success"] = "Item removed.";
        return RedirectToAction(nameof(Index));
    }
}