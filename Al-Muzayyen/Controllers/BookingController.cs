using Al_Muzayyen.Models;
using Al_Muzayyen.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Al_Muzayyen.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IClassService _classService;
        private readonly IPlaceService _placeService;
        private readonly ISlotService _slotService;

        // حقن السيرفسز المنفصلة
        public BookingController(
            IBookingService bookingService,
            IClassService classService,
            IPlaceService placeService,
            ISlotService slotService)
        {
            _bookingService = bookingService;
            _classService = classService;
            _placeService = placeService;
            _slotService = slotService;
        }

        [HttpGet]
        public async Task<IActionResult> booking()
        {
            ViewBag.Classes = await _classService.GetAllClassesAsync();
            ViewBag.Places = await _placeService.GetAllPlacesAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> booking(Booking booking)
        {
            // شرط إضافي: لو الطالب لم يختار الصف أو المكان أو المجموعة (قيمتهم بـ 0)
            if (!ModelState.IsValid || booking.ClassId == 0 || booking.PlaceId == 0 || booking.SlotId == 0)
            {
                ViewBag.ErrorMessage = "برجاء اختيار الصف الدراسي، المكان، والمجموعة المتاحة بشكل صحيح.";

                // إعادة ملء القوائم حتى لا تظهر فارغة
                ViewBag.Classes = await _classService.GetAllClassesAsync();
                ViewBag.Places = await _placeService.GetAllPlacesAsync();
                return View(booking);
            }

            var result = await _bookingService.CreateBookingAsync(booking);
            if (result)
            {
                ViewBag.SuccessMessage = "تم الحجز بنجاح!";
                return RedirectToAction(nameof(booking));
            }

            ViewBag.ErrorMessage = "حدث خطأ أثناء حفظ الحجز.";
            ViewBag.Classes = await _classService.GetAllClassesAsync();
            ViewBag.Places = await _placeService.GetAllPlacesAsync();
            return View(booking);
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableSlots(int classId, int placeId)
        {
            var slots = await _slotService.GetSlotsByFilterAsync(classId, placeId);
            var result = slots.Select(s => new { id = s.Id, name = s.Group_Name }).ToList();
            return Json(result);
        }
    }
}