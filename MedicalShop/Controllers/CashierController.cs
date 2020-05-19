using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using Data.Models;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
                if (role.Contains("i"))
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
                SendEmail(transaction);
            }
            return Json(new { data = transaction });
        }

        public void SendEmail(TransactionDetail transaction)
        {
            try
            {
                var file = ReportTransaction(transaction);
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("furqon2993@gmail.com");
                mail.To.Add("achmdfurqon@gmail.com");
                mail.Subject = "Transaction Report";
                mail.Body = "File Report is in the attachment below.";
                Attachment attachment = new Attachment(file, "report.pdf");
                mail.Attachments.Add(attachment);
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("furqon2993@gmail.com", "39919020");
                smtp.Send(mail);
            }
            catch
            {
                
            }            
        }

        public MemoryStream ReportTransaction(TransactionDetail transaction)
        {
            #region Declaration
            int maxColumn = 5;
            PdfPTable pdf = new PdfPTable(maxColumn);
            MemoryStream memory = new MemoryStream();
            Document document = new Document();
            document.SetPageSize(PageSize.A4);
            document.SetMargins(30f, 30f, 30f, 30f);
            pdf.WidthPercentage = 100;
            pdf.HorizontalAlignment = Element.ALIGN_LEFT;
            Font font = FontFactory.GetFont("Arial", 8f, 1);
            PdfWriter writer = PdfWriter.GetInstance(document, memory);
            document.Open();
            pdf.SetWidths(new float[] { 5f, 80f, 40f, 30f, 50f });
            #endregion
            #region Header
            font = FontFactory.GetFont("Tahoma", 11f, 1);
            PdfPCell cell = new PdfPCell(new Phrase("Transaction", font));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.ExtraParagraphSpace = 0;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + transaction.Code, font));
            cell.Colspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.ExtraParagraphSpace = 0;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            font = FontFactory.GetFont("Tahoma", 9f, 1);
            cell = new PdfPCell(new Phrase("Date", font));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.ExtraParagraphSpace = 2;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + transaction.Date, font));
            cell.Colspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.ExtraParagraphSpace = 2;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            #endregion
            #region Body
            font = FontFactory.GetFont("Tahoma", 8f, 0);
            int number = 1;
            foreach (var report in transaction.Sales)
            {
                cell = new PdfPCell(new Phrase(number++.ToString(), font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.Product.Name, font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.Product.Price.ToString(), font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.Quantity.ToString(), font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                cell = new PdfPCell(new Phrase(report.SubTotal.ToString(), font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = BaseColor.WHITE;
                pdf.AddCell(cell);
                pdf.CompleteRow();
            }
            cell = new PdfPCell(new Phrase("", font));
            cell.Colspan = 5;
            cell.Border = 0;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.CompleteRow();
            cell = new PdfPCell(new Phrase("", font));
            cell.Colspan = 3;
            cell.Border = 0;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Total", font));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase(transaction.TotalPrice.ToString(), font));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            cell = new PdfPCell(new Phrase("", font));
            cell.Colspan = 3;
            cell.Border = 0;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Cash", font));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase(transaction.Cash.ToString(), font));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            cell = new PdfPCell(new Phrase("", font));
            cell.Colspan = 3;
            cell.Border = 0;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase("Change", font));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            cell = new PdfPCell(new Phrase(transaction.Change.ToString(), font));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BackgroundColor = BaseColor.WHITE;
            pdf.AddCell(cell);
            pdf.CompleteRow();
            #endregion
            pdf.HeaderRows = 2;
            document.Add(pdf);
            writer.CloseStream = false;
            document.Close();
            memory.Position = 0;
            return memory;
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

        public JsonResult GetTransactionById(int id)
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
            return Json(new { data = transaction.Sales });
        }
        #endregion

    }
}