using Al_Muzayyen.Models;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;

namespace Al_Muzayyen.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenericService<Class> _classService;
        private readonly IGenericService<Booking> _bookingService;
        private readonly IGenericService<Place> _placeService;
        private readonly IGenericService<Video> _videoService;



        public AdminController(IGenericService<Class> classService, IGenericService<Booking> bookingService, IGenericService<Place> placeService, IGenericService<Video> videoService)
        {
            _classService = classService;
            _bookingService = bookingService;
            _placeService = placeService;
            _videoService = videoService;
        }   
        public IActionResult Index()
        {
            var model = new IndexVMAdmin
            {
                StudentsCount = _bookingService.GetAll().Count(),
                GroupsCount = _placeService.GetAll().Count(),
                ClassesCount = _classService.GetAll().Count()
            };

           
            return View(model);
        }

        public IActionResult Students()
        {
            return View();
        }

        public IActionResult Groups()
        {
            return View();
        }

        public IActionResult Locations()
        {
            return View();
        }

        public IActionResult Videos()
        {
            var model = _videoService.GetAll();

            return View(model);
        }
        [HttpPost]
        public IActionResult EditVideo(Video video)
        
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "البيانات غير صحيحة.";
                    return RedirectToAction("Videos");
                }

                _videoService.Update(video);
                _videoService.SaveChanges();

                TempData["Success"] = "تم تعديل الفيديو بنجاح.";
                return RedirectToAction("Videos");
            }
            catch (Exception)
            {
                TempData["Error"] = "حدث خطأ أثناء حفظ البيانات.";
                return View(video);
            }
        }

    }
}
