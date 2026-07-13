using Al_Muzayyen.Models;

namespace Al_Muzayyen.Repositories
{
    public interface IBookingRepo:IGenericRepository<Booking>
    {
        public  Task<List<Booking>> GetEnteredStudents();
    }
}
