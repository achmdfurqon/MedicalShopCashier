using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ITransactionRepository
    {
        Task<DataTableTransactions> GetTransactions(string keyword, int page, int size);
        Task<IEnumerable<TransactionDetail>> GetTransactions();
        Task<IEnumerable<TransactionDetail>> SearchTransactions(string keyword);
        TransactionDetail GetTransaction(int id);
        int AddTransaction(Transaction transaction);
        Task<IEnumerable<Report>> ReportByDate(DateRange date);
    }

    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetOrders();
        IEnumerable<SaleReport> GetOrderReport();
        int AddOrder(SaleVM sale);
        int CancelOrder(int id);
    }

    public interface IPurchaseRepository
    {
        Task<IEnumerable<Purchase>> GetPurchases();
        int AddPurchase(PurchaseVM purchase);
    }
}
