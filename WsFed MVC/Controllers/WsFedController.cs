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
    public async Task<IActionResult> Logout(int cleanup = 0)
    {
        await HttpContext.SignOutAsync();
        var tenant = Configuration["auth:wsfed:Tenant"]!;
        var appid = Configuration["auth:wsfed:AppId"]!;
        var waparameter = cleanup == 0 ? "wsignout1.0" : "wsignoutcleanup1.0";
        var postLogoutRedirect = Configuration["auth:wsfed:PostLogoutRedirect"]!;
        var idpHost = Configuration["auth:wsfed:IdpHost"]!;
        var logoutUrl = $"{idpHost}/WsFed/{tenant}/{appid}?wa={waparameter}&wreply={Uri.EscapeDataString(postLogoutRedirect)}";
        return Redirect(logoutUrl);
    }
}