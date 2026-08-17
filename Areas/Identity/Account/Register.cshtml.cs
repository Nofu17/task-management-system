using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "«·»—Ìœ «·≈·ﬂ —Ê‰Ì „ÿ·Ê»")]
            [EmailAddress(ErrorMessage = "’Ì€… «·»—Ìœ «·≈·ﬂ —Ê‰Ì €Ì— ’ÕÌÕ…")]
            public string Email { get; set; }

            [Required(ErrorMessage = "ﬂ·„… «·„—Ê— „ÿ·Ê»…")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = " √ﬂÌœ ﬂ·„… «·„—Ê— „ÿ·Ê»")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "ﬂ·„… «·„—Ê— Ê √ﬂÌœÂ« €Ì— „ ÿ«»ﬁ Ì‰.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // «· Õﬁﬁ „‰  ﬂ—«— «·»—Ìœ
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "«·»—Ìœ «·≈·ﬂ —Ê‰Ì „” Œœ„ „”»ﬁ«.");
                    return Page();
                }

                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("/Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
