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
        private readonly IPlaceService _placeService; 
        private readonly IGenericService<Video> _videoService;
        private readonly IGenericService<Available_slot> _available_slot;



        public AdminController(IGenericService<Class> classService, IGenericService<Booking> bookingService, IPlaceService placeService, IGenericService<Video> videoService, IGenericService<Available_slot> available_slot)
        {
            _classService = classService;
            _bookingService = bookingService;
            _placeService = placeService;
            _videoService = videoService;
            _available_slot = available_slot;
        }   
        public IActionResult Index()
        {
            var model = new IndexVMAdmin
            {
                StudentsCount = _bookingService.GetAll().Count(),
                GroupsCount = _available_slot.GetAll().Count(),
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

        public async Task<IActionResult> Locations()
        {
            // استخدام الدالة الحقيقية من السيرفيس
            var model = await _placeService.GetAllPlacesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(Place place)
        {
            // فحص إذا كان اسم المكان فارغاً
            if (string.IsNullOrEmpty(place.Name))
            {
                TempData["Error"] = "اسم المكان مطلوب ولا يمكن تركه فارغاً.";
                return RedirectToAction("Locations");
            }

            // تأمين القائمة العكسية منعاً لأي مشاكل في الـ Validation
            place.AvailableSlots = new List<Available_slot>();

            // حفظ في الداتابيز
            var result = await _placeService.CreatePlaceAsync(place);

            if (result)
            {
                TempData["Success"] = "تم إضافة المكان بنجاح.";
            }
            else
            {
                TempData["Error"] = "حدث خطأ أثناء حفظ المكان في الداتابيز.";
            }

            // إعادة التوجيه لصفحة الأماكن لعمل ريلود وعرض البيانات الحقيقية
            return RedirectToAction("Locations");
        }
        [HttpPost]
        public async Task<IActionResult> EditLocation(Place place)
        {
            if (place.Id == 0 || string.IsNullOrEmpty(place.Name))
            {
                TempData["Error"] = "بيانات المكان غير صالحة للتعديل.";
                return RedirectToAction("Locations");
            }

            try
            {
                place.AvailableSlots = new List<Available_slot>();

                // استخدام الدالة الجديدة المخصصة للتحديث
                var result = await _placeService.UpdatePlaceAsync(place);

                if (result)
                {
                    TempData["Success"] = "تم تعديل المكان بنجاح.";
                }
                else
                {
                    TempData["Error"] = "لم يتم تعديل أي بيانات.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "حدث خطأ أثناء تعديل البيانات.";
            }

            return RedirectToAction("Locations");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            if (id == 0)
            {
                TempData["Error"] = "معرف المكان غير صحيح.";
                return RedirectToAction("Locations");
            }

            try
            {
                var result = await _placeService.DeletePlaceAsync(id);
                if (result)
                {
                    TempData["Success"] = "تم حذف المكان بنجاح.";
                }
                else
                {
                    TempData["Error"] = "لم يتم العثور على المكان أو فشل الحذف.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "حدث خطأ أثناء الحذف (قد يكون المكان مرتبطاً بمجموعات حالية).";
            }

            return RedirectToAction("Locations");
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
