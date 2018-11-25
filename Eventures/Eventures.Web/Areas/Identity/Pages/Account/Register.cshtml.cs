using Eventures.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            [StringLength(100, MinimumLength = 3)]
            [RegularExpression("[0-9a-zA-Z-_.*~]+")]
            public string Username { get; set; }
            
            [Display(Name = "First Name")]
            [StringLength(100, MinimumLength = 4)]
            public string FirstName { get; set; }
            
            [Display(Name = "Last Name")]
            [StringLength(100, MinimumLength = 4)]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "UCN")]
            [StringLength(10, MinimumLength = 10, ErrorMessage = "UCN must be exactly {2} digits long.")]
            public string UCN { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [RegularExpression(".+")]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
            public string Password { get; set; }

            [Required]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.Username, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName, UniqueCitizenNumber = Input.UCN };
                var result = await _userManager.CreateAsync(user, Input.Password);

                var addRoleSuccess = await _userManager.AddToRoleAsync(user, "User");

                if (!addRoleSuccess.Succeeded)
                {
                    return this.BadRequest();
                }

                if (_userManager.Users.Count() == 1)
                {
                    addRoleSuccess = await _userManager.AddToRoleAsync(user, "Admin");

                    if (!addRoleSuccess.Succeeded)
                    {
                        return this.BadRequest();
                    }
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
