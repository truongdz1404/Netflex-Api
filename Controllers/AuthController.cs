using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Netflex.Entities;
using Netflex.Services.Implements;
using System.ComponentModel.DataAnnotations;
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
    private readonly IEmailSender _emailSender;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailSender = emailSender;
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
            return Ok(new { message = "If the email exists, an OTP has been sent" });

        var otp = new Random().Next(100000, 999999).ToString();

        string html = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Mã OTP Reset Password</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }}
        
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background: #ffffff;
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        
        .header {{
            background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }}
        
        .header h1 {{
            font-size: 28px;
            font-weight: 700;
            margin-bottom: 10px;
        }}
        
        .header p {{
            font-size: 16px;
            opacity: 0.9;
        }}
        
        .content {{
            padding: 50px 30px;
            text-align: center;
        }}
        
        .greeting {{
            font-size: 20px;
            color: #374151;
            margin-bottom: 30px;
            font-weight: 600;
        }}
        
        .message {{
            font-size: 16px;
            color: #6b7280;
            margin-bottom: 40px;
            line-height: 1.6;
        }}
        
        .otp-container {{
            background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
            border-radius: 16px;
            padding: 40px 20px;
            margin: 30px 0;
            border: 2px solid #e5e7eb;
        }}
        
        .otp-label {{
            font-size: 14px;
            color: #9ca3af;
            text-transform: uppercase;
            letter-spacing: 1px;
            font-weight: 600;
            margin-bottom: 15px;
        }}
        
        .otp-code {{
            font-size: 42px;
            font-weight: 800;
            color: #4f46e5;
            letter-spacing: 8px;
            text-shadow: 0 2px 4px rgba(79, 70, 229, 0.2);
            margin: 15px 0;
            font-family: 'Courier New', monospace;
        }}
        
        .otp-border {{
            width: 200px;
            height: 4px;
            background: linear-gradient(90deg, #4f46e5, #7c3aed);
            margin: 20px auto;
            border-radius: 2px;
        }}
        
        .warning {{
            background: #fef3cd;
            border: 1px solid #fbbf24;
            border-radius: 12px;
            padding: 20px;
            margin: 30px 0;
            color: #92400e;
        }}
        
        .warning-icon {{
            font-size: 20px;
            margin-bottom: 10px;
        }}
        
        .warning p {{
            font-size: 14px;
            margin: 8px 0;
        }}
        
        .footer {{
            background: #f9fafb;
            padding: 30px;
            text-align: center;
            border-top: 1px solid #e5e7eb;
        }}
        
        .footer p {{
            font-size: 14px;
            color: #6b7280;
            margin: 5px 0;
        }}
        
        .security-note {{
            background: #ecfdf5;
            border: 1px solid #10b981;
            border-radius: 12px;
            padding: 20px;
            margin: 20px 0;
            color: #047857;
        }}
        
        .security-note .icon {{
            font-size: 18px;
            margin-bottom: 8px;
        }}
        
        @media (max-width: 600px) {{
            .email-container {{
                margin: 10px;
                border-radius: 12px;
            }}
            
            .content {{
                padding: 30px 20px;
            }}
            
            .otp-code {{
                font-size: 36px;
                letter-spacing: 6px;
            }}
            
            .header {{
                padding: 30px 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>🔐 Xác thực tài khoản</h1>
            <p>Mã OTP để đặt lại mật khẩu</p>
        </div>
        
        <div class='content'>
            <div class='greeting'>
                Xin chào! 👋
            </div>
            
            <div class='message'>
                Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã OTP bên dưới để tiếp tục:
            </div>
            
            <div class='otp-container'>
                <div class='otp-label'>Mã xác thực OTP</div>
                <div class='otp-code'>{otp}</div>
                <div class='otp-border'></div>
            </div>
            
            <div class='warning'>
                <div class='warning-icon'>⚠️</div>
                <p><strong>Lưu ý quan trọng:</strong></p>
                <p>• Mã này sẽ hết hạn sau <strong>5 phút</strong></p>
                <p>• Không chia sẻ mã này với bất kỳ ai</p>
                <p>• Chỉ sử dụng mã này trên website chính thức của chúng tôi</p>
            </div>
            
            <div class='security-note'>
                <div class='icon'>🛡️</div>
                <p><strong>Bảo mật tài khoản:</strong> Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này và liên hệ với chúng tôi ngay lập tức.</p>
            </div>
        </div>
        
        <div class='footer'>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
            <p>© 2025 - Hệ thống được bảo mật bởi công nghệ hiện đại</p>
            <p style='color: #9ca3af; font-size: 12px; margin-top: 15px;'>
                Nếu bạn gặp vấn đề, vui lòng liên hệ support@netflex.com
            </p>
        </div>
    </div>
</body>
</html>";

        await _emailSender.SendEmailAsync(request.Email, "Mã OTP đặt lại mật khẩu", html);

        return Ok(new
        {
            message = "OTP has been sent to your email",
            otp = otp
        });
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid request" });
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (result.Succeeded)
            return Ok(new { message = "Password reset successfully" });

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return BadRequest(ModelState);
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (!string.Equals(user.Email, request.CurrentEmail, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Current email does not match" });

        var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordCheck)
            return BadRequest(new { message = "Invalid password" });

        var existingUser = await _userManager.FindByEmailAsync(request.NewEmail);
        if (existingUser != null && existingUser.Id != user.Id)
            return BadRequest(new { message = "Email already exists" });

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

        var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, token);

        if (result.Succeeded)
        {
            await _userManager.SetUserNameAsync(user, request.NewEmail);

            try
            {
                await _emailSender.SendEmailAsync(
                    request.NewEmail,
                    "Email Changed Successfully",
                    $"Your email has been successfully changed to {request.NewEmail}");
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                // _logger.LogError(ex, "Failed to send email notification");
            }

            return Ok(new { message = "Email changed successfully" });
        }

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
    public string NewPassword { get; set; } = string.Empty;
}


public class ChangeEmailRequest
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [EmailAddress]
    public string CurrentEmail { get; set; }

    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }

    [Required]
    public string Password { get; set; }
}
