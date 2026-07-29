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


        // 1. فتح الصفحة الأساسية فورا خفيفة جداً
        public IActionResult Index()
        {
            return View();
        }

        // 2. API جلب صورة الأستاذ
        [HttpGet]
        public IActionResult GetHeroImage()
        {
            var img = _adminService.GetAll().FirstOrDefault()?.ImageUrl ?? "/images/image.png";
            return Json(new { linkimage = img });
        }

        // 3. API المراحل الدراسية
        [HttpGet]
        public IActionResult GetClasses()
        {
            var classes = _classService.GetAll()
                .Select(c => new { c.Id, c.Name }); // DTO خفيف
            return Json(classes);
        }

        // 4. API أماكن السناتر
        [HttpGet]
        public IActionResult GetPlaces()
        {
            var places = _placeService.GetAll()
                .Select(p => new { p.Id, p.Name, p.Address });
            return Json(places);
        }

        // 5. API الفيديوهات
        [HttpGet]
        public IActionResult GetVideos()
        {
            var videos = _videoService.GetAll()
                .Select(v => new { v.Id, v.Title, v.Description, v.URL });
            return Json(videos);
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
