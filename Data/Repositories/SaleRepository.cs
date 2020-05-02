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
    public class SaleRepository : ISaleRepository
    {
        private readonly SqlConnection sql;
        DynamicParameters param = new DynamicParameters();
        public SaleRepository(ConnectionStrings connection)
        {
            sql = new SqlConnection(connection.Value);
        }

        public int AddOrder(SaleVM sale)
        {
            var sp = "SP_AddOrder";
            param.AddDynamicParams(sale);
            var result = sql.Execute(sp, param, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int CancelOrder(int id)
        {
            var sp = "SP_CancelOrder";
            param.Add("Id", id);
            var result = sql.Execute(sp, param, commandType: CommandType.StoredProcedure);
            return result;
        }

        public Sale GetOrder(int id)
        {
            var sp = "SP_GetOrder";
            param.Add("Id", id);
            var result = sql.Query<Sale, Product, Sale>(
                sp, map: (o, p) =>
                {
                    o.Product = p;
                    return o;
                },
                param: param,
                splitOn: "ProductId",
                commandType: CommandType.StoredProcedure
            ).SingleOrDefault();
            return result;
        }

        public async Task<IEnumerable<Sale>> GetOrders()
        {
            var sp = "SP_GetOrders";
            var result = await sql.QueryAsync<Sale, Product, Sale>(
                sp, map: (o, p) =>
                {
                    o.Product = p;
                    return o;
                },
                splitOn: "ProductId",
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}
