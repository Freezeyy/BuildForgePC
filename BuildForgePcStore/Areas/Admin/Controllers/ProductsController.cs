using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Models.Entities;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthorize]
public class ProductsController : Controller
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxImageBytes = 2 * 1024 * 1024;

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Category.CategoryName)
            .ThenBy(p => p.ProductName)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesDropdown();
        return View(new ProductEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditViewModel model)
    {
        await PopulateCategoriesDropdown(model.CategoryId);
        await NormalizeSocketForCategory(model);

        if (!ModelState.IsValid)
            return View(model);

        var product = new Product
        {
            ProductName = model.ProductName.Trim(),
            CategoryId = model.CategoryId,
            Description = model.Description.Trim(),
            Price = model.Price,
            Stock = model.Stock,
            Socket = model.Socket,
            ImagePath = "/images/products/placeholder.svg"
        };

        if (model.ImageFile != null)
        {
            var path = await SaveImageAsync(model.ImageFile);
            if (path == null)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Invalid image. Use JPG, PNG, or WEBP under 2 MB.");
                return View(model);
            }
            product.ImagePath = path;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Product created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound();

        await PopulateCategoriesDropdown(product.CategoryId);
        return View(MapToViewModel(product));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
    {
        if (id != model.ProductId) return NotFound();

        await PopulateCategoriesDropdown(model.CategoryId);
        await NormalizeSocketForCategory(model);

        if (!ModelState.IsValid)
            return View(model);

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.ProductName = model.ProductName.Trim();
        product.CategoryId = model.CategoryId;
        product.Description = model.Description.Trim();
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.Socket = model.Socket;

        if (model.ImageFile != null)
        {
            var path = await SaveImageAsync(model.ImageFile);
            if (path == null)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Invalid image. Use JPG, PNG, or WEBP under 2 MB.");
                model.CurrentImagePath = product.ImagePath;
                return View(model);
            }
            product.ImagePath = path;
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Product updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null) return NotFound();

        return View(MapToViewModel(product));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesDropdown(int? selectedId = null)
    {
        var categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName", selectedId);
        ViewBag.CategoryNames = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
    }

    private static ProductEditViewModel MapToViewModel(Product product) => new()
    {
        ProductId = product.ProductId,
        ProductName = product.ProductName,
        CategoryId = product.CategoryId,
        CategoryName = product.Category.CategoryName,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        Socket = product.Socket,
        CurrentImagePath = product.ImagePath
    };

    private async Task NormalizeSocketForCategory(ProductEditViewModel model)
    {
        var category = await _context.Categories.FindAsync(model.CategoryId);
        if (category == null) return;

        model.CategoryName = category.CategoryName;
        if (!RequiresSocket(category.CategoryName))
        {
            model.Socket = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(model.Socket))
            ModelState.AddModelError(nameof(model.Socket), "Socket is required for CPU and Motherboard.");
    }

    private static bool RequiresSocket(string categoryName) =>
        categoryName.Equals("CPU", StringComparison.OrdinalIgnoreCase) ||
        categoryName.Equals("Motherboard", StringComparison.OrdinalIgnoreCase);

    private async Task<string?> SaveImageAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext) || file.Length <= 0 || file.Length > MaxImageBytes)
            return null;

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/products/{fileName}";
    }
}
