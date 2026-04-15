using AuthX.Data;
using AuthX.Models;
using AuthX.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages;

public class ForgotPasswordModel : PageModel
{
    private readonly AppDbContext _db;

    public ForgotPasswordModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
    public string GeneratedToken { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email);

        Message = "If the account exists, a reset token was generated.";

        if (user == null)
            return Page();

        var reset = new PasswordResetToken
        {
            UserId = user.Id,
            Token = $"{user.Email}-reset",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            Used = false
        };

        _db.PasswordResetTokens.Add(reset);
        await _db.SaveChangesAsync();

        GeneratedToken = reset.Token;
        return Page();
    }
}