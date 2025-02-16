using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<IdentityUser> userManager;

    public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest();
        }

        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return NotFound();
        }

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToRole(string userName, string roleName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            return NotFound();
        }

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            return NotFound();
        }

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveUserFromRole(string userName, string roleName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            return NotFound();
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            return BadRequest();
        }

        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoles(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        return Ok(await Task.FromResult(roleManager.Roles));
    }
}
