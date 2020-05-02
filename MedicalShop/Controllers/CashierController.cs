using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data.Models;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MedicalShop.Controllers
{
    public class CashierController : Controller
    {
        private readonly HttpClient httpClient = new HttpClient();
        public CashierController()
        {
            httpClient.BaseAddress = new Uri("https://localhost:44317/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        //[Authorize(Roles = "Cashier")]
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                if (role.Contains("Cashier") || role.Contains("Admin"))
                {
                    ViewBag.Products = Products();
                    return View();
                }
                else
                {
                    return RedirectToAction("Error403", "Error");
                }
            }
            else
            {
                return RedirectToAction("", "Users");
            }
        }

        #region Orders
        public JsonResult GetOrders()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<Sale> orders = null;
            var responseTask = httpClient.GetAsync("Sales");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Sale>>();
                readTask.Wait();
                orders = readTask.Result;
            }
            var totalprice = orders.Sum(o => o.SubTotal);
            var totalproduct = orders.Count();
            return Json(new { data = orders, total = totalprice });
        }

        public IList<Product> Products()
        {            
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<Product> products = null;
            var responseTask = httpClient.GetAsync("Products");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Product>>();
                readTask.Wait();
                products = readTask.Result;
            }
            return products;
        }

        // POST: Products/Create
        public ActionResult CreateOrder(SaleVM order)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                var myContent = JsonConvert.SerializeObject(order);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var affectedRow = httpClient.PostAsync("Sales", byteContent).Result;
                return Json(new { data = affectedRow, affectedRow.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Delete/5
        public ActionResult CancelOrder(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var affectedRow = httpClient.DeleteAsync("Sales/" + id).Result;
            return Json(new { data = affectedRow });
        }
        #endregion

        #region Transaction
        public ActionResult CreateTransaction(Transaction transaction)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));           
                var myContent = JsonConvert.SerializeObject(transaction);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var affectedRow = httpClient.PostAsync("Transactions", byteContent).Result;
                return Json(new { data = affectedRow, affectedRow.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetTransaction()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            TransactionDetail transaction = null;
            var responseTask = httpClient.GetAsync("Transactions");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<TransactionDetail>>();
                readTask.Wait();
                var transactions = readTask.Result;
                transaction = transactions.LastOrDefault();
            }
            return Json(new { data = transaction });
        }

        public IActionResult Transactions()
        {
            return View();
        }

        public JsonResult GetTransactions()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<TransactionDetail> transactions = null;
            var responseTask = httpClient.GetAsync("Transactions");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<TransactionDetail>>();
                readTask.Wait();
                transactions = readTask.Result;
            }
            return Json(new { data = transactions });
        }

        public JsonResult GetTransaction(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            TransactionDetail transaction = null;
            var responseTask = httpClient.GetAsync("Transactions/" + id);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<TransactionDetail>();
                readTask.Wait();
                transaction = readTask.Result;
            }
            return Json(new { data = transaction });
        }
        #endregion

    }
}