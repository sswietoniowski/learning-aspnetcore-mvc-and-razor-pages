using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BulkyBook.MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [MaxLength(64)]
            [Display(Name = "Name")]
            public string Name { get; set; }
            [MaxLength(128)]
            [Display(Name = "Street")]
            public string StreetAddress { get; set; }
            [MaxLength(64)]
            [Display(Name = "City")]
            public string City { get; set; }
            [MaxLength(64)]
            [Display(Name = "State")]
            public string State { get; set; }
            [MaxLength(16)]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }
            [MaxLength(16)]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
            [MaxLength(64)]
            [Display(Name = "Role")]
            public string Role { get; set; }
            [Display(Name = "Company")]
            public int? CompanyId { get; set; }

            public IEnumerable<SelectListItem> CompaniesList { get; set; }
            public IEnumerable<SelectListItem> RolesList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            Input = new InputModel
            {
                CompaniesList = _unitOfWork.Companies.GetAll()
                    .Select(item => new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }),
                RolesList = _roleManager.Roles
                    .Where(r => r.Name != SD.IdentityRole_Customer_Individual)
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name
                    })
            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    PhoneNumber = Input.PhoneNumber,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    State = Input.State,
                    PostalCode = Input.PostalCode,
                    Role = Input.Role,
                    CompanyId = Input.CompanyId
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(SD.IdentityRole_Customer_Individual))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Customer_Individual));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.IdentityRole_Customer_Company))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Customer_Company));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.IdentityRole_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.IdentityRole_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Employee));
                    }

                    if (String.IsNullOrWhiteSpace(user.Role))
                    {
                        await _userManager.AddToRoleAsync(user, SD.IdentityRole_Customer_Individual);
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.IdentityRole_Customer_Company);
                        }
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, ReturnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(user.Role))
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            // admin is registering a new user we don't want him to logout
                            return RedirectToAction("Index", "Users", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Input = new InputModel
            {
                CompaniesList = _unitOfWork.Companies.GetAll()
                    .Select(item => new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }),
                RolesList = _roleManager.Roles
                    .Where(r => r.Name != SD.IdentityRole_Customer_Individual)
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name
                    })
            };

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
