using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Services
{
    public interface IGenericService<T> where T : class
    {
        List<T> GetAll();

        void Update(T entity);
        void Delete(T entity);

        void SaveChanges(); 


    }
}
