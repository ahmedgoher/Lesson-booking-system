using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;

namespace Al_Muzayyen.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IGenericRepository<Place> _placeRepo;
        public PlaceService(IGenericRepository<Place> placeRepo) => _placeRepo = placeRepo;

        public async Task<IEnumerable<Place>> GetAllPlacesAsync() => await _placeRepo.GetAllAsync();
        public async Task<bool> CreatePlaceAsync(Place newPlace) { await _placeRepo.AddAsync(newPlace); return await _placeRepo.SaveChangesAsync(); }

        
    }
}
