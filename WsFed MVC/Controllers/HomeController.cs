using Microsoft.AspNetCore.Mvc;

namespace WsFed_MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Dashboard()
    {
        return View();
    }
}