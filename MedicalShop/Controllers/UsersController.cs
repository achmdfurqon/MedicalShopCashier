using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MedicalShop.Controllers
{
    public class UsersController : Controller
    {
        readonly HttpClient httpClient = new HttpClient();
        public UsersController()
        {
            httpClient.BaseAddress = new Uri("https://localhost:44317/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                var role = HttpContext.Session.GetString("Role");
                if (role.Contains("Manager"))
                {
                    return RedirectToAction("", "Manager");
                }
                else if (role.Contains("Admin"))
                {
                    return RedirectToAction("", "Admin");
                }
                else if(role.Contains("Cashier"))
                {
                    return RedirectToAction("", "Cashier");
                }
                else
                {
                    return RedirectToAction("", "Home");
                }
            }
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost("Users/Register")]
        public ActionResult Register(UserVM userVM)
        {
            var myContent = JsonConvert.SerializeObject(userVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var register = httpClient.PostAsync("Users/Register", byteContent).Result;
            if (register.IsSuccessStatusCode)
            {
                var readTask = register.Content.ReadAsAsync<User>().Result;
                return Json(new { data = readTask, register.StatusCode });
            }
            return Json(new { register.StatusCode });
        }

        // POST: Login
        [HttpPost("Users/Login")]
        public ActionResult Login(UserVM userVM)
        {
            var myContent = JsonConvert.SerializeObject(userVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var login = httpClient.PostAsync("Users/Login", byteContent).Result;
            if (login.IsSuccessStatusCode)
            {
                var readTask = login.Content.ReadAsAsync<UserVM>().Result;
                HttpContext.Session.SetString("UserId", readTask.Id);
                HttpContext.Session.SetString("Username", readTask.UserName);
                HttpContext.Session.SetString("Name", readTask.Name);
                HttpContext.Session.SetString("Role", string.Join(' ', readTask.Role));
                HttpContext.Session.SetString("Token", "Bearer " + readTask.Token);
                httpClient.DefaultRequestHeaders.Add("Authorization", readTask.Token);
                return Json(new { data = readTask, login.StatusCode });
            }
            return Json(new { login.StatusCode });
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("", "Users");
            }
            else
            {
                HttpContext.Session.Remove("UserId");
                HttpContext.Session.Remove("Username");
                HttpContext.Session.Remove("Name");
                HttpContext.Session.Remove("Role");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}