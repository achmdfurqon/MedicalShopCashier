using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MedicalShop.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error401()
        {
            return View();
        }

        public IActionResult Error403()
        {
            return View();
        }

        [Route("[controller]/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("[controller]/500")]
        public IActionResult Error500()
        {
            return View();
        }
    }
}