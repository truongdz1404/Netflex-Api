// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Netflex.Entities;

namespace Netflex.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager,
        ILogger<LoginModel> logger,
        UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }


        [TempData]
        public string ErrorMessage { get; set; }


        public class InputModel
        {

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                Console.WriteLine($"Is Admin: {isAdmin}");

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");

                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            return LocalRedirect("~/Admin");
                        }
                        foreach (var role in roles)
                        {
                            Console.WriteLine($"User Role: {role}");
                        }

                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        ModelState.AddModelError(string.Empty, "Tài khoản đã bị vô hiệu hoá.");
                        // return RedirectToPage("./Lockout");
                        return Page();
                    }
                }

                ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu.");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostExternalLogin(string provider)
        {
            var redirectUrl = Url.Page("Account/ExternalLogin", pageHandler: "Callback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user != null && await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị vô hiệu hoá.");
                return Page();
            }

            return new ChallengeResult(provider, properties);
        }

    }
}
