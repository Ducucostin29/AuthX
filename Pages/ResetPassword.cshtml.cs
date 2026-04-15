using System.ComponentModel.DataAnnotations;
using AuthX.Data;
using AuthX.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages;

public class ResetPasswordModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwordService;

    public ResetPasswordModel(AppDbContext db, PasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    [BindProperty]
    public string Token { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var reset = await _db.PasswordResetTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == Token);

        if (reset == null || reset.User == null || reset.Used || reset.ExpiresAtUtc < DateTime.UtcNow)
        {
            Message = "Invalid or expired token.";
            return Page();
        }

        reset.User.PasswordHash = _passwordService.HashPassword(reset.User, NewPassword);
        reset.Used = true;

        await _db.SaveChangesAsync();

        Message = "Password reset successful.";
        return Page();
    }
}