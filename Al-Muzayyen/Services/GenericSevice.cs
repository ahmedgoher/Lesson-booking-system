using Al_Muzayyen.Repositories;

namespace Al_Muzayyen.Services
{
    public class GenericSevice<T> : IGenericService<T> where T : class 
    {
        public IGenericRepository<T> Repository { get; set; }
        public GenericSevice(IGenericRepository<T> genericRepository)
        {
            Repository = genericRepository;
        }

        public List<T> GetAll()
        {
           return Repository.GetAll();
        }
        public void Add(T entity)
        {
            Repository.Add(entity);
        }

        public void Update(T entity)
        {
            Repository.Update(entity);
        }

        public void Delete(T entity)
        {
            Repository.Delete(entity);

        }

        public void SaveChanges()
        {
            Repository.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await Repository.SaveChangesAsync();
        }
    }
}
