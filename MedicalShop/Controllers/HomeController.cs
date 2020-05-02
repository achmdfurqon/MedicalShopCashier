using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicalShop.Models;
using Microsoft.AspNetCore.Http;

namespace MedicalShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                if (HttpContext.Session.GetString("Role").Contains("Manager"))
                {
                    return RedirectToAction("", "Manager");
                }
                else if(HttpContext.Session.GetString("Role").Contains("Cashier"))
                {
                    return RedirectToAction("", "Cashier");
                }
                else if (HttpContext.Session.GetString("Role").Contains("Admin"))
                {
                    return RedirectToAction("", "Admin");
                }
                else
                {
                    return RedirectToAction("", "Users");
                }
            }
            else
            {
                return RedirectToAction("", "Users");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
