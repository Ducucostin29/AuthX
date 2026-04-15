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
            Message = "User does not exist";
            return Page();
        }

        if (!_passwordService.VerifyPassword(user, Input.Password))
        {
            Message = "Wrong password";
            return Page();
        }
        var valid = _passwordService.VerifyPassword(user, Input.Password);

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
            principal);

        return RedirectToPage("/Dashboard");
    }
}