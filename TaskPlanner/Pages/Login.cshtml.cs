using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace TaskPlanner.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        public IStringLocalizer<LoginModel> Localizer { get; }
        public LoginModel(IStringLocalizer<LoginModel> localizer)
        {
            Localizer = localizer;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();
            HttpContext.Session.SetString("Username", Username);
            return RedirectToPage("Index");
        }
    }
} 