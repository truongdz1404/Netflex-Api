using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Netflex.Areas.Identity.Pages.Account
{
    [Route("[controller]")]
    public class ExternalAuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ExternalAuthController> _logger;

        public ExternalAuthController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ExternalAuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "ExternalAuth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            try
            {
                if (remoteError != null || Request.Query["error"].Count > 0)
                {
                    _logger.LogWarning($"External login error: {remoteError ?? Request.Query["error"]}");
                    TempData["ErrorMessage"] = "Đăng nhập không thành công.";
                    return RedirectToAction("Login", "Account");
                }

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogError("Lỗi external login info");
                    return RedirectToAction("Login", "Account");
                }

                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email) ?? throw new Exception("User email is null"));
                    if (user != null)
                    {
                        await LogUserRole(user);
                        return RedirectToRoleBasedPage(user, returnUrl);
                    }
                }
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true,
                        };

                        var createResult = await _userManager.CreateAsync(user);
                        if (createResult.Succeeded)
                        {
                            if (!await _roleManager.RoleExistsAsync("User"))
                            {
                                await _roleManager.CreateAsync(new IdentityRole("User"));
                            }
                            await _userManager.AddToRoleAsync(user, "User");

                            await _userManager.AddLoginAsync(user, info);
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            await LogUserRole(user); 
                            return RedirectToRoleBasedPage(user, returnUrl);
                        }
                    }
                    else
                    {
                        await _userManager.AddLoginAsync(user, info);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await LogUserRole(user); 
                        return RedirectToRoleBasedPage(user, returnUrl);
                    }
                }

                return RedirectToAction("Login", "Account");
            }
            catch (AuthenticationFailureException ex)
            {
                _logger.LogWarning($"External login failed: {ex.Message}");
                TempData["ErrorMessage"] = "Đăng nhập không thành công.";
                return RedirectToAction("Login", "Account");
            }
        }

        private async Task LogUserRole(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleList = string.Join(", ", roles);

            Console.WriteLine($"[AUTH] User {user.Email} có các role: {roleList}");

            _logger.LogInformation($"[AUTH] User {user.Email} có các role: {roleList}");
        }


        private IActionResult RedirectToRoleBasedPage(User user, string? returnUrl)
        {
            if (user == null) return RedirectToAction("Login", "Account");

            var returnUrlFinal = returnUrl ?? "/";
            return User.IsInRole("Admin") ? RedirectToAction("Dashboard", "Admin") : LocalRedirect(returnUrlFinal);
        }
    }
}
