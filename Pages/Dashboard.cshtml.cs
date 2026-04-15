using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthX.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    public void OnGet()
    {
    }
}