using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WsFed_MVC.Controllers;

[AllowAnonymous]
[Route("WsFed")]
public class WsFedController : Controller
{
    private IConfiguration Configuration { get; }

    public WsFedController(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    [Route("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/Home/Dashboard" }, "WsFederation");
    }

    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        var idpLogoutOutUrl = Configuration["auth:wsfed:IdPLogoutUrl"]!;
        var postLogoutRedirect = $"{Request.Scheme}://{Request.Host}/Home/Index";
        var logoutUrl = $"{idpLogoutOutUrl}&wreply={Uri.EscapeDataString(postLogoutRedirect)}";
        return Redirect(logoutUrl);
    }
}