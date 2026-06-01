using BuildForgePcStore.Data;
using BuildForgePcStore.Helpers;
using BuildForgePcStore.Models;
using BuildForgePcStore.Models.Entities;
using BuildForgePcStore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildForgePcStore.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email))
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        var user = new User
        {
            Name = model.Name.Trim(),
            Email = email,
            Role = UserRoles.Customer
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        HttpContext.Session.SetUserSession(user.UserId, user.Name, user.Role);
        TempData["Success"] = "Welcome! Your account has been created.";
        return RedirectToLocal(null);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToLocal(returnUrl);

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        HttpContext.Session.SetUserSession(user.UserId, user.Name, user.Role);
        TempData["Success"] = $"Welcome back, {user.Name}!";
        return RedirectToLocal(returnUrl);
    }

    /// <summary>Customer account home (after login).</summary>
    [HttpGet]
    public IActionResult Index()
    {
        if (!HttpContext.Session.IsLoggedIn())
            return RedirectToAction(nameof(Login));

        if (HttpContext.Session.IsAdmin())
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        ViewBag.UserName = HttpContext.Session.GetUserName();
        return View();
    }

    [HttpGet("/logout")]
    [HttpGet("/Account/Logout")]
    public IActionResult Logout()
    {
        return PerformLogout();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Logout")]
    public IActionResult LogoutPost()
    {
        return PerformLogout();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        if (HttpContext.Session.IsAdmin())
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        return RedirectToAction(nameof(Index));
    }

    private IActionResult PerformLogout()
    {
        HttpContext.Session.ClearUserSession();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }
}
