using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data.Models;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using MedicalShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MedicalShop.Controllers
{
    public class ManagerController : Controller
    {
        private readonly HttpClient httpClient = new HttpClient();
        public int cost = 0;
        public ManagerController()
        {
            httpClient.BaseAddress = new Uri("https://localhost:44317/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        //[Authorize(Roles = "Manager")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                if (role.Contains("Manager"))
                {
                    ViewBag.Products = Products();
                    ViewBag.Suppliers = Suppliers();
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

        public JsonResult GetPurchases()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<Purchase> purchases = null;
            var responseTask = httpClient.GetAsync("Purchases");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Purchase>>();
                readTask.Wait();
                purchases = readTask.Result;
            }
            cost = purchases.Sum(s => s.Cost);
            return Json(new { data = purchases, total = cost });
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
            httpClient.DefaultRequestHeaders.Clear();
            return products;
        }

        public JsonResult ProductAutocomplete()
        {
            IList<ProductModel> products = Products()
                .Select(p => new ProductModel { data = p.Id, value = p.Name }).ToList();            
            return Json(new { data = products });
        }

        public IList<Supplier> Suppliers()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<Supplier> products = null;
            var responseTask = httpClient.GetAsync("Suppliers");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Supplier>>();
                readTask.Wait();
                products = readTask.Result;
            }
            httpClient.DefaultRequestHeaders.Clear();
            return products;
        }

        public JsonResult SupplierAutocomplete()
        {
            IList<ProductModel> supplier = Suppliers()
                .Select(p => new ProductModel { data = p.Id, value = p.Name }).ToList();
            return Json(new { data = supplier });
        }

        // POST: Products/Create
        public ActionResult CreatePurchase(PurchaseVM purchase)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                var myContent = JsonConvert.SerializeObject(purchase);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var affectedRow = httpClient.PostAsync("Purchases", byteContent).Result;
                return Json(new { data = affectedRow, affectedRow.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        #region Transaction
        public IActionResult Transaction()
        {
            return View();
        }
        public JsonResult GetTransactions()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<Transaction> transactions = null;
            var responseTask = httpClient.GetAsync("Transactions");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Transaction>>();
                readTask.Wait();
                transactions = readTask.Result;
            }
            return Json(new { data = transactions });
        }
        #endregion
    }
}