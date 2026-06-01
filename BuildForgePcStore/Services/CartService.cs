using System.Text.Json;
using BuildForgePcStore.Data;
using BuildForgePcStore.Helpers;
using BuildForgePcStore.Models;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace BuildForgePcStore.Services;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    private ISession Session => _httpContextAccessor.HttpContext!.Session;
    public List<CartItem> GetItems()
    {
        var json = Session.GetString(SessionKeys.Cart);
        if (string.IsNullOrEmpty(json)) return new List<CartItem>();
        return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }
    private void SaveItems(List<CartItem> items) =>
    Session.SetString(SessionKeys.Cart, JsonSerializer.Serialize(items));
    public int GetItemCount() => GetItems().Sum(i => i.Quantity);
    public void AddItem(int productId, int quantity)
    {
        if (quantity < 1) quantity = 1;
        var items = GetItems();
        var existing = items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
            existing.Quantity += quantity;
        else
            items.Add(new CartItem { ProductId = productId, Quantity = quantity });
        SaveItems(items);
    }
    public void UpdateQuantity(int productId, int quantity)
    {
        var items = GetItems();
        var item = items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;
        if (quantity <= 0)
            items.Remove(item);
        else
            item.Quantity = quantity;
        SaveItems(items);
    }
    public void RemoveItem(int productId)
    {
        var items = GetItems().Where(i => i.ProductId != productId).ToList();
        SaveItems(items);
    }
    public void Clear() => Session.Remove(SessionKeys.Cart);
    public async Task<CartViewModel> BuildCartViewModelAsync()
    {
        var items = GetItems();
        var vm = new CartViewModel();
        if (!items.Any()) return vm;
        var ids = items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
        .Include(p => p.Category)
        .Where(p => ids.Contains(p.ProductId))
        .ToListAsync();
        foreach (var cartItem in items)
        {
            var product = products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
            if (product == null) continue;
            var line = product.Price * cartItem.Quantity;
            vm.Lines.Add(new CartLineViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ImagePath = product.ImagePath,
                UnitPrice = product.Price,
                Quantity = cartItem.Quantity,
                LineTotal = line,
                Stock = product.Stock
            });
        }
        return vm;
    }
}