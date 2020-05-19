using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
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
                if (role.Contains("Admin"))
                {
                    return RedirectToAction("", "Admin");
                }
                else if(role.Contains("Manager"))
                {
                    return RedirectToAction("", "Manager");
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
            var register = httpClient.PostAsync("Account/Register", byteContent).Result;
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
            var login = httpClient.PostAsync("Account/Login", byteContent).Result;
            if (login.IsSuccessStatusCode)
            {
                var readTask = login.Content.ReadAsAsync<UserVM>().Result;
                HttpContext.Session.SetString("UserId", readTask.Id);
                HttpContext.Session.SetString("Username", readTask.UserName);
                HttpContext.Session.SetString("Name", readTask.Name);
                HttpContext.Session.SetString("Role", readTask.Role);
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

        [Route("ForgetPassword/{id}")]
        public ActionResult ForgetPassword(string id)
        {
            ViewBag.UserId = id;
            return View();
        }

        public ActionResult SendLink(string email)
        {
            try
            {
                var get = httpClient.GetAsync("Account/" + email).Result;
                var user = get.Content.ReadAsAsync<UserVM>().Result;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("furqon2993@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Change Password";
                mail.IsBodyHtml = true;
                mail.Body = "Please click this <a href='https://localhost:44387/ForgetPassword/" + user.Id + "'>link</a> to change your password.";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("furqon2993@gmail.com", "39919020");
                smtp.Send(mail);
                return Json(new { data = true });
            }
            catch
            {
                return Json(new { data = false });
            }
        }

        public JsonResult ForgetPassword(string id, string password)
        {
            try
            {
                var userVM = new UserVM(); userVM.PasswordHash = password;
                var myContent = JsonConvert.SerializeObject(userVM);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var ByteContent = new ByteArrayContent(buffer);
                ByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var update = httpClient.PutAsync("Account/ForgetPassword/" + id, ByteContent).Result;
                return Json(new { update.StatusCode });
            }
            catch
            {
                return Json(new { statusCode = 500 });
            }
        }
    }
}