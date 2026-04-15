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

    public RegisterModel(AppDbContext db, PasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
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

        Message = "Account created successfully.";
        return Page();
    }
}