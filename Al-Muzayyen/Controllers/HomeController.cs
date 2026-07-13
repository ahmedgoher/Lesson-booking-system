using Al_Muzayyen.Models;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Al_Muzayyen.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericService<Class> _classService;
        private readonly IGenericService<Place> _placeService;
        private readonly IGenericService<Video> _videoService;
        private readonly IGenericService<Admin> _adminService;

        public HomeController(IGenericService<Admin> adminService, IGenericService<Video> videoService, IGenericService<Class> classService, IGenericService<Place> placeService)
        {

            _adminService = adminService;
            _videoService = videoService;
            _classService = classService;
            _placeService = placeService;
            
        }


        public IActionResult Index()
        {

            var model = new HomeVM
            {

                linkimage= _adminService.GetAll().FirstOrDefault()?.ImageUrl ?? "/images/image.png",
                classes = _classService.GetAll(),
                places = _placeService.GetAll(),
                LinkesVideos = _videoService.GetAll()



            };
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
