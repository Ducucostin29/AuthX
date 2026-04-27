using System.Security.Claims;
using AuthX.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthX.Pages;

public class LogoutModel : PageModel
{
    private readonly AuditService _audit;

    public LogoutModel(AuditService audit)
    {
        _audit = audit;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _audit.LogAsync("LOGOUT", "auth", null, userId);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToPage("/Login");
    }
}