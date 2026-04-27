using System.ComponentModel.DataAnnotations;
using AuthX.Data;
using AuthX.Models;
using AuthX.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwordService;
    private readonly AuditService _audit;

    public RegisterModel(AppDbContext db, PasswordService passwordService, AuditService audit)
    {
        _db = db;
        _passwordService = passwordService;
        _audit = audit;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string Message { get; set; } = "";

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Parola trebuie să aibă minim 8 caractere, o literă mare, una mică și o cifră.")]
        public string Password { get; set; } = "";
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var exists = await _db.Users.AnyAsync(x => x.Email == Input.Email);

        if (exists)
        {
            await _audit.LogAsync("REGISTER_FAILED_EMAIL_EXISTS", "auth", null, null);
            Message = "User already exists.";
            return Page();
        }

        var user = new User
        {
            Email = Input.Email,
            Role = "USER"
        };

        user.PasswordHash = _passwordService.HashPassword(user, Input.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("REGISTER_SUCCESS", "auth", null, user.Id.ToString());

        Message = "Account created successfully.";
        return Page();
    }
}