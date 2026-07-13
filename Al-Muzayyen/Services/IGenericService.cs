using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Services
{
    public interface IGenericService<T> where T : class
    {
        List<T> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        void SaveChanges(); 


    }
}
