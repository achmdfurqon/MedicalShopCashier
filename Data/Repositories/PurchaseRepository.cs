using Dapper;
using Data.Interfaces;
using Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly SqlConnection sql;
        DynamicParameters param = new DynamicParameters();
        public PurchaseRepository(ConnectionStrings connection)
        {
            sql = new SqlConnection(connection.Value);
        }

        public int AddPurchase(PurchaseVM purchase)
        {
            var sp = "SP_AddPurchase";
            param.AddDynamicParams(purchase);
            var result = sql.Execute(sp, param, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases()
        {
            var sp = "SP_GetPurchases";
            var result = await sql.QueryAsync<Purchase, Product, Supplier, Purchase>(
                sp, map: (b, p, s) =>
                {
                    b.Product = p;
                    b.Supplier = s;
                    return b;
                }, 
                splitOn: "ProductId, SupplierId",
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}
