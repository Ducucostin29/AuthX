using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AuthX.Data;
using AuthX.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwordService;

    public LoginModel(AppDbContext db, PasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string Message { get; set; } = string.Empty;

    public class InputModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);

        if (user == null)
        {
            Message = "Invalid credentials";
            return Page();
        }

        if (user.LockUntilUtc.HasValue && user.LockUntilUtc > DateTime.UtcNow)
        {
            Message = "Account temporarily locked. Try again later.";
            return Page();
        }

        var valid = _passwordService.VerifyPassword(user, Input.Password);

        if (!valid)
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockUntilUtc = DateTime.UtcNow.AddMinutes(10);
                user.FailedLoginAttempts = 0;
            }

            await _db.SaveChangesAsync();

            Message = "Invalid credentials";
            return Page();
        }

        user.FailedLoginAttempts = 0;
        user.LockUntilUtc = null;
        await _db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            });

        return RedirectToPage("/Dashboard");
    }
}