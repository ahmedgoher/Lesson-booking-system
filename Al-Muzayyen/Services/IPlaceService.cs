using Al_Muzayyen.Models;

namespace Al_Muzayyen.Services
{
    public interface IPlaceService
    {
        Task<IEnumerable<Place>> GetAllPlacesAsync();
        Task<bool> CreatePlaceAsync(Place newPlace); // للآدمن
        Task<bool> UpdatePlaceAsync(Place place);
        Task<bool> DeletePlaceAsync(int id);


    }
}