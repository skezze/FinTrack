using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        //it has info from jwt into middlewares
        await _signInManager.SignOutAsync();
        return Ok();
    }
}
