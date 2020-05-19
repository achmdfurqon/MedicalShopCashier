using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data.Models;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MedicalShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MedicalShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient httpClient = new HttpClient();
        public AdminController()
        {
            httpClient.BaseAddress = new Uri("https://localhost:44317/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                if (role.Contains("Admin"))
                {
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

        #region Dashboard
        public JsonResult PieChartJS()
        {
            ChartJS chart = null;
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var responseTask = httpClient.GetAsync("Products");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IEnumerable<Product>>();
                readTask.Wait();
                var list = readTask.Result;
                chart = new ChartJS(list.Count());
                list = list.OrderByDescending(o => o.Stock);
                int i = 0;
                foreach (var report in list)
                {
                    chart.labels[i] = report.Name;
                    chart.datasets[0].data[i] = report.Stock;
                    i++;
                }
            }
            return Json(new { data = chart });
        }

        public JsonResult EChartPie()
        {
            List<EChart> charts = new List<EChart>();
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var responseTask = httpClient.GetAsync("Products");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IEnumerable<Product>>();
                readTask.Wait();
                var list = readTask.Result;
                foreach (var report in list)
                {
                    EChart chart = new EChart();
                    chart.Name = report.Name;
                    chart.Value = report.Stock;
                    charts.Add(chart);
                }
            }
            return Json(new { data = charts, legend = charts.Select(c=>c.Name).ToArray() });
        }

        public JsonResult EChartLine()
        {
            //var charts = new List<IGrouping<string, EChart>>();
            var charts = new List<EChart>();            
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var responseTask = httpClient.GetAsync("Sales/All");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IEnumerable<SaleReport>>();
                readTask.Wait();
                var list = readTask.Result;
                charts = list.Select(s => new EChart { Name = s.Name, Value = s.Qty }).ToList();                
                //charts = list.Select(s => new EChart { Name = s.Product.Name, Value = s.Quantity }).GroupBy(g => g.Name).ToList();                
            }
            return Json(new { valueX = charts.Select(c => c.Name).ToArray(), valueY = charts.Select(c => c.Value).ToArray() });
        }
        #endregion

        #region Product
        public IActionResult Products()
        {
            return View();
        }
        // GET : Products/Lists
        public async Task<DataTableProducts> GetProducts(string keyword, int size, int page)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));            
            var url = "Products/data?keyword=" + keyword + "&page=" + page + "&size=" + size;
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<DataTableProducts>();
            }
            return null;
        }

        [HttpGet("Products/Data")]
        public IActionResult DataProduct(IDataTablesRequest request)
        {
            var pageSize = request.Length;
            var pageNumber = request.Start / request.Length + 1;
            string keyword = string.Empty;
            if (request.Search.Value != null)
            {
                keyword = request.Search.Value;
            }
            var dataPage = GetProducts(keyword, pageSize, pageNumber).Result;
            var response = DataTablesResponse.Create(request, dataPage.length, dataPage.filterLength, dataPage.data);
            return new DataTablesJsonResult(response, true);
        }

        public IList<Product> ProductsList()
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

        // GET: Products/Details/5
        public JsonResult GetProduct(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var cek = httpClient.GetAsync("Products/" + id).Result;
            var read = cek.Content.ReadAsAsync<Product>().Result;
            return Json(new { data = read });
        }
        // POST: Products/Create
        public ActionResult CreateProduct(Product product)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                var myContent = JsonConvert.SerializeObject(product);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var affectedRow = httpClient.PostAsync("Products", byteContent).Result;
                return Json(new { data = affectedRow, affectedRow.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        // POST: Products/Edit/5
        public ActionResult EditProduct(int id, Product product)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                var myContent = JsonConvert.SerializeObject(product);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var ByteContent = new ByteArrayContent(buffer);
                ByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var update = httpClient.PutAsync("Products/" + id, ByteContent).Result;
                return Json(new { data = update, update.StatusCode });
            }
            catch
            {
                return View();
            }
        }
        // GET: Products/Delete/5
        public ActionResult DeleteProduct(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var affectedRow = httpClient.DeleteAsync("Products/" + id).Result;
            return Json(new { data = affectedRow });
        }

        public ActionResult RemoveProduct(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var affectedRow = httpClient.GetAsync("Products/Remove/" + id).Result;
            return Json(new { data = affectedRow });
        }
        #endregion

        #region Supplier
        public IActionResult Suppliers()
        {
            return View();
        }
        // GET : Suppliers/Lists
        public async Task<DataTableSuppliers> GetSuppliers(string keyword, int size, int page)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var url = "Suppliers/data?keyword=" + keyword + "&page=" + page + "&size=" + size;
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<DataTableSuppliers>();
            }
            return null;
        }

        [HttpGet("Suppliers/Data")]
        public IActionResult DataSupplier(IDataTablesRequest request)
        {
            var pageSize = request.Length;
            var pageNumber = request.Start / request.Length + 1;
            string keyword = string.Empty;
            if (request.Search.Value != null)
            {
                keyword = request.Search.Value;
            }
            var dataPage = GetSuppliers(keyword, pageSize, pageNumber).Result;
            var response = DataTablesResponse.Create(request, dataPage.length, dataPage.filterLength, dataPage.data);
            return new DataTablesJsonResult(response, true);
        }

        //public IList<Supplier> Suppliers()
        //{
        //    var UserId = HttpContext.Session.GetString("UserId");
        //    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
        //    IList<Supplier> suppliers = null;
        //    var responseTask = httpClient.GetAsync("Products/report/" + UserId);
        //    responseTask.Wait();
        //    var result = responseTask.Result;
        //    if (result.IsSuccessStatusCode)
        //    {
        //        var readTask = result.Content.ReadAsAsync<IList<Supplier>>();
        //        readTask.Wait();
        //        suppliers = readTask.Result;
        //    }
        //    return suppliers;
        //}

        // GET: Suppliers/Details/5
        public JsonResult GetSupplier(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var cek = httpClient.GetAsync("Suppliers/" + id).Result;
            var read = cek.Content.ReadAsAsync<Supplier>().Result;
            return Json(new { data = read });
        }
        // POST: Suppliers/Create
        public ActionResult CreateSupplier(Supplier supplier)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                var myContent = JsonConvert.SerializeObject(supplier);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var affectedRow = httpClient.PostAsync("Suppliers", byteContent).Result;
                return Json(new { data = affectedRow, affectedRow.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        // POST: Products/Edit/5
        public ActionResult EditSupplier(int id, Supplier supplier)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                var myContent = JsonConvert.SerializeObject(supplier);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var ByteContent = new ByteArrayContent(buffer);
                ByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var update = httpClient.PutAsync("Suppliers/" + id, ByteContent).Result;
                return Json(new { data = update, update.StatusCode });
            }
            catch
            {
                return View();
            }
        }
        // GET: Products/Delete/5
        public ActionResult DeleteSupplier(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var affectedRow = httpClient.DeleteAsync("Suppliers/" + id).Result;
            return Json(new { data = affectedRow });
        }

        public ActionResult RemoveSupplier(int id)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var affectedRow = httpClient.GetAsync("Suppliers/Remove/" + id).Result;
            return Json(new { data = affectedRow });
        }
        #endregion

        public ActionResult ExportPDF()
        {
            List<Product> list = ProductsList().ToList();
            #region declaration
            int maxColumn = 5;
            PdfPTable pdf = new PdfPTable(maxColumn);
            MemoryStream memory = new MemoryStream();
            #endregion
            #region
            Document document = new Document();
            document.SetPageSize(PageSize.A4);
            document.SetMargins(30f, 30f, 30f, 30f);
            pdf.WidthPercentage = 100;
            pdf.HorizontalAlignment = Element.ALIGN_LEFT;
            Font font = FontFactory.GetFont("Arial", 8f, 1);
            PdfWriter.GetInstance(document, memory);
            document.Open();
            pdf.SetWidths(new float[] { 10f, 80f, 40f, 30f, 150f });
            #endregion
            #region Header
            font = FontFactory.GetFont("Tahoma", 11f, 1);
            PdfPCell cell = new PdfPCell(new Phrase("Products List", font));
            cell.Colspan = maxColumn;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.ExtraParagraphSpace = 0;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            font = FontFactory.GetFont("Tahoma", 9f, 1);
            cell = new PdfPCell(new Phrase("", font));
            cell.Colspan = maxColumn;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.ExtraParagraphSpace = 2;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            #endregion
            #region Body
            #region Table Header
            font = FontFactory.GetFont("Tahoma", 8f, 1);
            cell = new PdfPCell(new Phrase("No", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Name", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Price", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Stock", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Description", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            #endregion
            #region Table Body
            font = FontFactory.GetFont("Tahoma", 8f, 0);
            int number = 1;
            foreach (var product in list)
            {
                cell = new PdfPCell(new Phrase(number++.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(product.Name, font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(product.Price.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(product.Stock.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(product.Description, font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                pdf.CompleteRow();
            }
            #endregion
            #endregion
            pdf.HeaderRows = 2;
            document.Add(pdf);
            document.Close();
            byte[] file = memory.ToArray();
            //return File(file, "application/pdf");
            return File(
                fileContents: file,
                contentType: "application/pdf",
                fileDownloadName: "ProductsList.pdf");
        }

        public ActionResult ExportExcel()
        {
            List<Product> list = ProductsList().ToList();
            byte[] excel = null;
            using (var package = new ExcelPackage())
            {
                int maxColumn = 5;
                var worksheet = package.Workbook.Worksheets.Add("ProductsList");
                worksheet.Column(1).Width = 5;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 10;
                worksheet.Column(5).Width = 50;
                #region Title
                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells["A2:E2"].Merge = true;
                worksheet.Cells[1, 1].Value = "Products List";
                worksheet.Cells[1, 1].Style.Font.Size = 14;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value = "";
                worksheet.Cells[2, 1].Style.Font.Size = 14;
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                #endregion
                #region Header
                worksheet.Cells[3, 1].Value = "No";
                worksheet.Cells[3, 2].Value = "Name";
                worksheet.Cells[3, 3].Value = "Price";
                worksheet.Cells[3, 4].Value = "Stock";
                worksheet.Cells[3, 5].Value = "Description";
                for (int i = 1; i <= maxColumn; i++)
                {
                    worksheet.Cells[3, i].Style.Font.Bold = true;
                    worksheet.Cells[3, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                #endregion
                #region Body
                int number = 1;
                worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                foreach (var product in list)
                {
                    worksheet.Cells[number + 3, 1].Value = number;
                    worksheet.Cells[number + 3, 2].Value = product.Name;
                    worksheet.Cells[number + 3, 3].Value = product.Price;
                    worksheet.Cells[number + 3, 4].Value = product.Stock;
                    worksheet.Cells[number + 3, 5].Value = product.Description;
                    number++;
                }
                #endregion
                excel = package.GetAsByteArray();
            }
            if (excel == null || excel.Length == 0)
            {
                return NotFound();
            }
            return File(
                fileContents: excel,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "ProductsList.xlsx");
        }

        #region User
        public IActionResult Users()
        {
            ViewBag.Roles = GetRoles();
            return View();
        }

        public JsonResult GetUsers()
        {
            IList<UserVM> users = null;
            var responseTask = httpClient.GetAsync("Users");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<UserVM>>();
                readTask.Wait();
                users = readTask.Result;
            }
            return Json(new { data = users });
        }

        public List<IdentityRole> GetRoles()
        {
            List<IdentityRole> roles = null;
            var responseTask = httpClient.GetAsync("Users/Roles");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<IdentityRole>>();
                readTask.Wait();
                roles = readTask.Result.ToList();
            }
            return roles;
        }

        public JsonResult GetRole(string id)
        {
            var get = httpClient.GetAsync("Users/Role/" + id).Result;
            if (get.IsSuccessStatusCode)
            {
                ViewBag.Roles = get.Content.ReadAsAsync<IList<IdentityRole>>().Result;
            }
            return Json(new { get.StatusCode });
        }

        public ActionResult CreateUser(UserVM user)
        {
            try
            {
                ViewBag.Roles = GetRoles();
                var myContent = JsonConvert.SerializeObject(user);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var affectedRow = httpClient.PostAsync("Users", byteContent).Result;
                return Json(new { data = affectedRow, affectedRow.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AssignRole(string id, string role)
        {
            try
            {
                var update = httpClient.GetAsync("Users/AssignRole?id=" + id + "&role=" + role).Result;
                return Json(new { update.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult RemoveRole(string id, IdentityRole role)
        {
            try
            {
                var myContent = JsonConvert.SerializeObject(role);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var ByteContent = new ByteArrayContent(buffer);
                ByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var update = httpClient.PutAsync("Users/RemoveRole/" + id, ByteContent).Result;
                return Json(new { update.StatusCode });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteUser(string id)
        {
            var delete = httpClient.DeleteAsync("Users/" + id).Result;
            return Json(new { data = delete });
        }
        #endregion
    }
}