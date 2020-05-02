using Dapper;
using Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class SupplierRepository : Repository<Supplier>
    {
        private readonly ConnectionStrings connectionStrings;
        DynamicParameters parameters = new DynamicParameters();
        public SupplierRepository(ConnectionStrings connection) : base(connection)
        {
            connectionStrings = connection;
        }
        public async Task<DataTableSuppliers> Get(string keyword, int page, int size)
        {
            using (var sql = new SqlConnection(connectionStrings.Value))
            {
                var sp = "SP_GetSuppliers";
                var offset = (page - 1) * size;
                parameters.Add("Offset", offset);
                parameters.Add("PageSize", size);
                parameters.Add("Keyword", keyword);
                parameters.Add("@length", DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@filterLength", DbType.Int32, direction: ParameterDirection.Output);
                var result = new DataTableSuppliers();
                result.data = await sql.QueryAsync<Supplier>(sp, parameters, commandType: CommandType.StoredProcedure);
                result.length = parameters.Get<int>("@length");
                result.filterLength = parameters.Get<int>("@filterLength");
                return result;
            }
        }
    }
}
