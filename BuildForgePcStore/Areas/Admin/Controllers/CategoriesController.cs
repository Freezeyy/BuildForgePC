using BuildForgePcStore.Data;
using BuildForgePcStore.Filters;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthorize]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View(new CategoryEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var name = model.CategoryName.Trim();
        if (await _context.Categories.AnyAsync(c => c.CategoryName == name))
        {
            ModelState.AddModelError(nameof(model.CategoryName), "This category already exists.");
            return View(model);
        }

        _context.Categories.Add(new Models.Entities.Category { CategoryName = name });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return View(new CategoryEditViewModel
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryEditViewModel model)
    {
        if (id != model.CategoryId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        var name = model.CategoryName.Trim();
        if (await _context.Categories.AnyAsync(c => c.CategoryName == name && c.CategoryId != id))
        {
            ModelState.AddModelError(nameof(model.CategoryName), "This category name is already used.");
            return View(model);
        }

        category.CategoryName = name;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Category updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return NotFound();

        ViewBag.ProductCount = category.Products.Count;
        return View(new CategoryEditViewModel
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName
        });
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return NotFound();

        if (category.Products.Any())
        {
            TempData["Error"] = "Cannot delete — products still use this category.";
            return RedirectToAction(nameof(Index));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Index));
    }
}
