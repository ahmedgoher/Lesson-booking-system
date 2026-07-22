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
        private readonly IGenericService<Admin> _admin;



        // حقن السيرفسز المنفصلة
        public BookingController(
            IGenericService<Admin> admin,
            IBookingService bookingService,
            IClassService classService,
            IPlaceService placeService,
            ISlotService slotService)
        {
            _admin = admin;
            _bookingService = bookingService;
            _classService = classService;
            _placeService = placeService;
            _slotService = slotService;
        }

        [HttpGet]
        public async Task<IActionResult> booking()
        {
            var phone = _admin.GetAll();
            string phonenumber;
            if (phone.Count == 0)
            {
                ViewBag.number = "";

            }
            else 
            {
                phonenumber = phone.FirstOrDefault().PhoneNumber;
                ViewBag.number = phonenumber;

            }

            ViewBag.Classes = await _classService.GetAllClassesAsync();
            ViewBag.Places = await _placeService.GetAllPlacesAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> booking(Student booking)
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
                // 1. نضع رسالة النجاح
                ViewBag.SuccessMessage = "🎉 تم تسجيل بياناتك وحجز الموعد بنجاح!";

                // 2. نعيد ملء القوائم عشان الصفحة متضربش وهي بتفتح تاني
                ViewBag.Classes = await _classService.GetAllClassesAsync();
                ViewBag.Places = await _placeService.GetAllPlacesAsync();

                // 3. نرجع الصفحة مباشرة (View) بدل الـ Redirect عشان الـ ViewBag يفضل عايش ويظهر
                return View(new Student()); // بعتنا كائن جديد فاضي عشان نفضي الفورم للطالب بعد النجاح
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

            var result = slots.Select(s => new {
                id = s.Id,
                name = s.SlotTimes != null && s.SlotTimes.Any()
                    ? $"{s.Group_Name} ( " + string.Join(" - ", s.SlotTimes.Select(t => $"{t.Day} الساعة {t.Time.ToString("hh:mm tt")}")) + " )"
                    : s.Group_Name // لو المواعيد مجتش لأي سبب يعرض اسم المجموعة فقط بدون أقواس فاضية
            }).ToList();

            return Json(result);
        }
    }
}