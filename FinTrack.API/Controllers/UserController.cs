using FinTrack.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase 
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly ApplicationDbContext dbContext;

    public UserController(
        UserManager<IdentityUser> userManager,
        ApplicationDbContext dbContext)
    {
        this.userManager = userManager;
        this.dbContext = dbContext;
    }
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await Task.FromResult(userManager.Users));
    }

        [HttpPost]
    public async Task<IActionResult> AddUser([FromBody]RegisterModel registerModel)
    {
        var result = await userManager.CreateAsync(new IdentityUser(registerModel.Username), registerModel.Password);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserByUserName(string userName) 
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is not null)
        {
            return Ok(user);
        }
        return NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody]UpdateUserModel model)
    {
        var user = await userManager.FindByNameAsync(model.CurrentUsername);
        if (user == null)
        {
            return NotFound();
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var setUserNameResult = await userManager.SetUserNameAsync(user, model.NewUsername);
            if (!setUserNameResult.Succeeded)
            {
                return BadRequest(setUserNameResult.Errors);
            }

            var setPasswordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!setPasswordResult.Succeeded)
            {
                return BadRequest(setPasswordResult.Errors);
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok();
        }
        catch
        {
            await transaction.RollbackAsync();
            return BadRequest("Credentials error");
        }
    }


    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return NotFound();
        }

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest();
    }
}