using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Helpers;
using BuildForgePcStore.Models.Entities;
using BuildForgePcStore.Models.ViewModels;
using BuildForgePcStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BuildForgePcStore.Controllers;

[CustomerAuthorize]
public class CheckoutController : Controller
{
    private const decimal TaxRate = 0.06m; // 6% SST
    private readonly ApplicationDbContext _context;
    private readonly CartService _cart;
    public CheckoutController(ApplicationDbContext context, CartService cart)
    {
        _context = context;
        _cart = cart;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cartVm = await _cart.BuildCartViewModelAsync();
        if (!cartVm.Lines.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }
        var subtotal = cartVm.Subtotal;
        var tax = Math.Round(subtotal * TaxRate, 2);
        return View(new CheckoutViewModel
        {
            Cart = cartVm,
            Subtotal = subtotal,
            TaxAmount = tax,
            TotalAmount = subtotal + tax
        });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var cartVm = await _cart.BuildCartViewModelAsync();
        if (!cartVm.Lines.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }
        model.Cart = cartVm;
        model.Subtotal = cartVm.Subtotal;
        model.TaxAmount = Math.Round(model.Subtotal * TaxRate, 2);
        model.TotalAmount = model.Subtotal + model.TaxAmount;

        if (!model.ConfirmOrder)
            ModelState.AddModelError(nameof(model.ConfirmOrder), "You must confirm your order.");

        if (!ModelState.IsValid)
            return View(model);
        var userId = HttpContext.Session.GetUserId();
        if (!userId.HasValue)
            return RedirectToAction("Login", "Account");
        foreach (var line in cartVm.Lines)
        {
            var product = await _context.Products.FindAsync(line.ProductId);
            if (product == null || product.Stock < line.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"Not enough stock for {line.ProductName}.");
                return View(model);
            }
        }
        var order = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.Now,
            Subtotal = model.Subtotal,
            TaxAmount = model.TaxAmount,
            TotalAmount = model.TotalAmount
        };
        foreach (var line in cartVm.Lines)
        {
            var product = await _context.Products.FindAsync(line.ProductId);
            if (product == null) continue;
            product.Stock -= line.Quantity;
            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = product.ProductId,
                Quantity = line.Quantity,
                UnitPrice = product.Price,
                Subtotal = product.Price * line.Quantity
            });
        }
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        _cart.Clear();
        return RedirectToAction(nameof(Receipt), new { id = order.OrderId });
    }
    [HttpGet]
    public async Task<IActionResult> Receipt(int? id)
    {
        if (id == null) return NotFound();
        var userId = HttpContext.Session.GetUserId();
        var isAdmin = HttpContext.Session.IsAdmin();
        var order = await _context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderDetails)
        .ThenInclude(d => d.Product)
        .FirstOrDefaultAsync(o => o.OrderId == id);
        if (order == null) return NotFound();
        if (!isAdmin && order.UserId != userId) return NotFound();
        var vm = new ReceiptViewModel
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            CustomerName = order.User.Name,
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