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
        public async Task<bool> UpdatePlaceAsync(Place place)
        {
            _placeRepo.Update(place);
            return await _placeRepo.SaveChangesAsync();
        }
        public async Task<bool> DeletePlaceAsync(int id)
        {
            // جلب المكان أولاً من خلال الـ Repo
            var place = await _placeRepo.GetByIdAsync(id);
            if (place == null) return false;

            _placeRepo.Delete(place); // أو اسم الدالة عندك في الـ GenericRepo (غالباً Delete أو Remove)
            return await _placeRepo.SaveChangesAsync();
        }

    }
}
