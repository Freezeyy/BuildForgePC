using BuildForgePcStore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildForgePcStore.Filters;

/// <summary>Requires an authenticated Admin user.</summary>
public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        if (!session.IsAdmin())
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
        }
    }
}
