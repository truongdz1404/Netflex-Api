using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Netflex.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Netflex.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            return Ok(new { message = "User registered successfully" });

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(new
        {
            message = "Login successful",
            user = new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName
            }
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Auth controller is working", timestamp = DateTime.Now });
    }

    [HttpGet("test-auth")]
    public IActionResult TestAuth([FromQuery] string userId)
    {
        return Ok(new
        {
            message = "Manual user test",
            userId = userId
        });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { message = "User ID is required" });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            userName = user.UserName,
            emailConfirmed = user.EmailConfirmed
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        user.UserName = request.UserName ?? user.UserName;
        user.Email = request.Email ?? user.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return Ok(new { message = "Profile updated successfully" });

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return BadRequest(ModelState);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
            return Ok(new { message = "Password changed successfully" });

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return BadRequest(ModelState);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Ok(new { message = "If the email exists, a password reset link has been sent" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return Ok(new { message = "Password reset token generated", token = token });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid request" });

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (result.Succeeded)
            return Ok(new { message = "Password reset successfully" });

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return BadRequest(ModelState);
    }
}


public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateProfileRequest
{
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
}

public class ChangePasswordRequest
{
    public string UserId { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
