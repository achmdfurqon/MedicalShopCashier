using Dapper;
using Data.Interfaces;
using Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SqlConnection sql;
        DynamicParameters param = new DynamicParameters();
        public TransactionRepository(ConnectionStrings connection)
        {
            sql = new SqlConnection(connection.Value);
        }

        public async Task<IEnumerable<TransactionDetail>> Transactions(string sp, DynamicParameters parameters)
        {
            var lookup = new Dictionary<int, TransactionDetail>();
            await sql.QueryAsync<TransactionDetail, Sales, Product, TransactionDetail>(sp,
               map: (t, s, p) =>
               {
                   TransactionDetail transaction;
                   if (!lookup.TryGetValue(t.Id, out transaction))
                   {
                       transaction = t;
                       transaction.Sales = new List<Sales>();
                       lookup.Add(transaction.Id, transaction);
                   }
                   s.Product = p;
                   transaction.Sales.Add(s);
                   return transaction;
               },
               param: parameters,
               splitOn: "SaleId, ProductId",
               commandType: CommandType.StoredProcedure);
            var result = lookup.Values.ToList();
            return result;
        }

        public TransactionDetail GetTransaction(int id)
        {
            var sp = "SP_GetTransaction";
            param.Add("Id", id);
            var result = Transactions(sp, param);
            return result.Result.SingleOrDefault();
        }        

        public async Task<IEnumerable<TransactionDetail>> GetTransactions()
        {
            var sp = "SP_GetTransactions";
            var lookup = new Dictionary<int, TransactionDetail>();
            await sql.QueryAsync<TransactionDetail, Sales, Product, TransactionDetail>(sp,
               map: (t, s, p) =>
             {
                 TransactionDetail transaction;
                 if (!lookup.TryGetValue(t.Id, out transaction))
                 {
                     transaction = t;
                     transaction.Sales = new List<Sales>();
                     lookup.Add(transaction.Id, transaction);
                 }
                 s.Product = p;    
                 transaction.Sales.Add(s);
                 return transaction;
             },
               splitOn: "SaleId, ProductId",                
               commandType: CommandType.StoredProcedure);
            var result = lookup.Values.ToList();
            return result;
        }

        public async Task<DataTableTransactions> GetTransactions(string keyword, int page, int size)
        {
            var sp = "SP_DataTransactions";
            var offset = (page - 1) * size;
            param.Add("Offset", offset);
            param.Add("PageSize", size);
            param.Add("Keyword", keyword);
            param.Add("@length", DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@filterLength", DbType.Int32, direction: ParameterDirection.Output);
            var result = new DataTableTransactions();          
            result.data = await Transactions(sp, param);
            result.length = param.Get<int>("@length");
            result.filterLength = param.Get<int>("@filterLength");
            return result;
        }

        public async Task<IEnumerable<TransactionDetail>> SearchTransactions(string keyword)
        {
            var sp = "SP_SearchTransactions";
            param.Add("Keyword", keyword);           
            var result = await Transactions(sp, param);
            return result;
        }

        public int AddTransaction(Transaction transaction)
        {
            var sp = "SP_AddTransaction";
            param.Add("Code", transaction.Code);
            param.Add("Date", transaction.Date);
            param.Add("Cash", transaction.Cash);
            var result = sql.Execute(sp, param, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
