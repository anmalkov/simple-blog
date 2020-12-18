using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SimpleBlog.Pages
{
    public class LoginModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private const string AccessCodeEnvironmentVariableName = "SIMPLEBLOG_ACCESSCODE";

        [BindProperty]
        [Required]
        [Display(Name = "Enter your code")]
        public string Code { get; set; }

        [FromQuery(Name = "ReturnUrl")]
        public string ReturnUrl { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/admin");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(AccessCodeEnvironmentVariableName)))
            {
                ModelState.AddModelError("Code", "The code is not configured, you can find how to configure it in the documentation");
                return Page();
            }

            if (Code != Environment.GetEnvironmentVariable(AccessCodeEnvironmentVariableName))
            {
                ModelState.AddModelError("Code", "The code is not correct");
                return Page();
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "admin"));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    
            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToPage("/admin");
        }
    }
}
