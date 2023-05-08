using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Repositories.Interfaces
{
    public interface IMongoRepository<T>
    {
        Task<bool> InsertAsync(T item);
        Task<T> GetByIdAsync(ObjectId id);
        Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter);
        Task UpdateAsync(ObjectId id, T item);
        Task<bool> DeleteAsync(ObjectId id);
        Task UpdateByUsernameAsync(string username, T item);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByUserNameAsync(string name);
    }
}
