using Dapper.Contrib.Extensions;
using Data.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly SqlConnection sql;
        public Repository(ConnectionStrings connection)
        {
            sql = new SqlConnection(connection.Value);
        }

        public Task<bool> DeleteAsync(TEntity entity)
        {
            var delete = sql.DeleteAsync(entity);
            return delete;
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            var getAll = await sql.GetAllAsync<TEntity>();
            return getAll;
        }

        public Task<TEntity> GetAsync(int id)
        {
            var get = sql.GetAsync<TEntity>(id);
            return get;
        }

        public Task<int> PostAsync(TEntity entity)
        {
            var post = sql.InsertAsync(entity);
            return post;
        }

        public Task<bool> PullAsync(TEntity entity)
        {
            var put = sql.UpdateAsync(entity);
            return put;
        }
    }
}
