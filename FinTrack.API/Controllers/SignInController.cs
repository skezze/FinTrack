using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using FinTrack.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]/[action]")]
[ApiController]
public class SignInController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public SignInController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var token = JwtHelper.GenerateJwtToken(user, _configuration);
        return Ok(token);
    }

    [HttpGet]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleCallback")
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return Unauthorized();

        var email = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        // 👉 Сюда добавь сохранение в БД при необходимости

        // Генерация JWT
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, email)
        };

        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("super_secret_key_12345")
        );

        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
            key, SecurityAlgorithms.HmacSha256
        );

        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwt);

        // Отдаём HTML, который вернёт токен через window.opener
        var html = $@"
            <html>
            <body>
                <script>
                    window.opener.postMessage({{ token: '{token}' }}, '*');
                    window.close();
                </script>
            </body>
            </html>
        ";

        return Content(html, "text/html");
    }
}
