// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using ToDoList.DataAccess.Repository.IRepository;
using ToDoList.Models;
using ToDoList.Utility;

namespace ToDoList.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public ApplicationUser ApplicationUser { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> Groups1 { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> Groups2 { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            //if (!_roleManager.RoleExistsAsync(StaticDependencies.Role_User_Owner).GetAwaiter().GetResult())
            //{
            //	_roleManager.CreateAsync(new IdentityRole(StaticDependencies.Role_User_Owner)).GetAwaiter().GetResult();
            //	_roleManager.CreateAsync(new IdentityRole(StaticDependencies.Role_User_Admin)).GetAwaiter().GetResult();
            //	_roleManager.CreateAsync(new IdentityRole(StaticDependencies.Role_User_Customer)).GetAwaiter().GetResult();
            //}
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Input = new()
            {
                Groups1 = await GetGroupsAsync(SubjectType.Standard),
                Groups2 = await GetGroupsAsync(SubjectType.English)
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.Nickname = Input.ApplicationUser.Nickname;
                user.Group1Id = Input.ApplicationUser.Group1Id;
                user.Group2Id = Input.ApplicationUser.Group2Id;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, SD.Role_User_Customer);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId, code, returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form

            Input.Groups1 = await GetGroupsAsync(SubjectType.Standard);
            Input.Groups2 = await GetGroupsAsync(SubjectType.English);

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }


		private async Task<IEnumerable<SelectListItem>> GetGroupsAsync(SubjectType subjectType)
		{
			switch (subjectType)
			{
				case SubjectType.Standard:
					IEnumerable<Group> groupList1 = await _unitOfWork.Group.GetAllAsync(filter: x => x.GroupType == GroupType.Standard || x.GroupType == GroupType.None, order: x => x.Name);
					IEnumerable<SelectListItem> groupItemList1 = groupList1.Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
					return groupItemList1;
				case SubjectType.English:
					IEnumerable<Group> groupList2 = await _unitOfWork.Group.GetAllAsync(filter: x => x.GroupType == GroupType.English || x.GroupType == GroupType.None, order: x => x.Name);
					IEnumerable<SelectListItem> groupItemList2 = groupList2.Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
					return groupItemList2;
				case SubjectType.Other:
					IEnumerable<Group> groupList3 = await _unitOfWork.Group.GetAllAsync(order: x => x.Name);
					IEnumerable<SelectListItem> groupItemList3 = groupList3.Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
					return groupItemList3;
				default:
					throw new Exception("Unknown subject type");
			}
		}
	}
}
