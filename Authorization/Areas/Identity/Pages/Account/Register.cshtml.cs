using Authorization.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Authorization.Areas.Identity.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUserModel> _signInManager;
		private readonly UserManager<ApplicationUserModel> _userManager;
		private readonly IUserStore<ApplicationUserModel> _userStore;
		private readonly IUserEmailStore<ApplicationUserModel> _emailStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public string ReturnUrl { get; set; }

		[BindProperty]
		public InputModel Input{ get; set;}

		public RegisterModel(SignInManager<ApplicationUserModel> signInManager,
			UserManager<ApplicationUserModel> userManager,
			IUserStore<ApplicationUserModel> userStore,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender
			)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_logger = logger;
			_emailSender = emailSender;
		}

		private IUserEmailStore<ApplicationUserModel> GetEmailStore()
		{
            if (!_userManager.SupportsUserEmail)
            {
				throw new NotSupportedException("The default UI requires a user store with email support.");
            }
			return (IUserEmailStore<ApplicationUserModel>)_userStore;
        }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			[Required]
			[DataType(DataType.Password)]
			[Display(Name = "password")]
			[StringLength(100,ErrorMessage = "The {0} must be at least {2} and at most {1} characters long", MinimumLength = 8)]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password",ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}
		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			// GetExternalAuthenticationSchemesAsync() for
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			// if returnUrl == null then set returnUrl = Url.Content("~/");
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = CreateUser();
				/*
				user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
				user.DateOfBirth = Input.DateOfBirth;
				*/

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
				var result = await _userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					var UserId = await _userManager.GetUserIdAsync(user);
					var EmailCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

					EmailCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(EmailCode));
					var callback = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new {area ="Identity", userId = UserId, emailCode = EmailCode, returnUrl = returnUrl },
						protocol: Request.Scheme
						);
					await _emailSender.SendEmailAsync(Input.Email, "Confirm your Email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callback)}'>Clicking here</a>.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = ReturnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
                    }
                }
				else
				{
					Console.WriteLine("message WASN'T soaded");
				}
				foreach(var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return Page();
		}

		private ApplicationUserModel CreateUser()
		{
			try
			{
				return Activator.CreateInstance<ApplicationUserModel>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUserModel)}'" 
					+ $"Ensure that '{nameof(ApplicationUserModel)}' is not an abstract class and has a parameterless constructor, or" +
					$"override the register page in Areas/Identity/Pages/Account/Register.cshtml");
			}
		}
	}
}
