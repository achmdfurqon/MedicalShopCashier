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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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
                if (role.Contains("n"))
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

        #region Report
        public IList<Report> ReportList(DateTime start, DateTime end)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            IList<Report> report = null;
            var url = "Transactions/Report?start=" + start + "&end=" + end;
            var responseTask = httpClient.GetAsync(url);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Report>>();
                readTask.Wait();
                report = readTask.Result;
            }
            return report;
        }

        [HttpGet("Report/Data")]
        public JsonResult Report(DateTime start, DateTime end)
        {
            IList<Report> report = ReportList(start, end);
            return Json(new { data = report });
        }

        public ActionResult ExportPDF(DateTime start, DateTime end)
        {
            List<Report> list = ReportList(start, end).ToList();
            #region declaration
            int maxColumn = 6;
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
            pdf.SetWidths(new float[] { 10f, 80f, 40f, 40f, 40f, 40f });
            #endregion
            #region Header
            font = FontFactory.GetFont("Tahoma", 11f, 1);
            PdfPCell cell = new PdfPCell(new Phrase("Report List", font));
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
            cell = new PdfPCell(new Phrase("Product Name", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Qty In", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Purchase", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Qty Out", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Sale", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            #endregion
            #region Table Body
            font = FontFactory.GetFont("Tahoma", 8f, 0);
            int number = 1;
            foreach (var report in list)
            {
                cell = new PdfPCell(new Phrase(number++.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.Product, font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.QtyIn.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.Purchase.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.QtyOut.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.Sale.ToString(), font));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
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
                fileDownloadName: "ReportList.pdf");
        }

        public ActionResult ExportExcel(DateTime start, DateTime end)
        {
            List<Report> list = ReportList(start, end).ToList();
            byte[] excel = null;
            using (var package = new ExcelPackage())
            {
                int maxColumn = 6;
                var worksheet = package.Workbook.Worksheets.Add("ProductsList");
                worksheet.Column(1).Width = 5;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 10;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 10;
                worksheet.Column(6).Width = 15;
                #region Title
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A2:F2"].Merge = true;
                worksheet.Cells[1, 1].Value = "Report List";
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
                worksheet.Cells[3, 2].Value = "Product Name";
                worksheet.Cells[3, 3].Value = "Qty In";
                worksheet.Cells[3, 4].Value = "Purchase";
                worksheet.Cells[3, 5].Value = "Qty Out";
                worksheet.Cells[3, 6].Value = "Sale";
                for (int i = 1; i <= maxColumn; i++)
                {
                    worksheet.Cells[3, i].Style.Font.Bold = true;
                    worksheet.Cells[3, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                #endregion
                #region Body
                int number = 1;
                worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                foreach (var report in list)
                {
                    worksheet.Cells[number + 3, 1].Value = number;
                    worksheet.Cells[number + 3, 2].Value = report.Product;
                    worksheet.Cells[number + 3, 3].Value = report.QtyIn;
                    worksheet.Cells[number + 3, 4].Value = report.Purchase;
                    worksheet.Cells[number + 3, 5].Value = report.QtyOut;
                    worksheet.Cells[number + 3, 6].Value = report.Sale;
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
                fileDownloadName: "ReportList.xlsx");
        }
        #endregion
    }
}