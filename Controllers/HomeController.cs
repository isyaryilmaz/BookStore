using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStoreMvc.Models;

namespace BookStoreMvc.Controllers;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        ViewBag.Title = "Anasayfa";
        return View();
    }

    // GET: /Home/About
    public ActionResult About()
    {
        ViewBag.Title = "Hakkýmýzda";
        return View();
    }

    // GET: /Home/Contact
    public ActionResult Contact()
    {
        ViewBag.Title = "Bize Ulaþýn";
        return View();
    }
}
   

