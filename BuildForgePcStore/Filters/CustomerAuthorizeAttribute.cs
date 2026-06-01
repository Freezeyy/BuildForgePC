using BuildForgePcStore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildForgePcStore.Filters;

/// <summary>Requires an authenticated Customer (or Admin).</summary>
public class CustomerAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        if (!session.IsLoggedIn())
        {
            context.Result = new RedirectToActionResult(
                "Login",
                "Account",
                new { area = "", returnUrl = context.HttpContext.Request.Path });
        }
    }
}
