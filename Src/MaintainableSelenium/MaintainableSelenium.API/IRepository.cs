using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaintainableSelenium.API
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(string id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}