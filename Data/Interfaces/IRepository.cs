using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T> GetAsync(int id);
        Task<int> PostAsync(T entity);
        Task<bool> PullAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
