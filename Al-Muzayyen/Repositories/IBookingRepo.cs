using Al_Muzayyen.Models;

namespace Al_Muzayyen.Repositories
{
    public interface IBookingRepo:IGenericRepository<Student>
    {
        public  Task<List<Student>> GetEnteredStudents();
    }
}
