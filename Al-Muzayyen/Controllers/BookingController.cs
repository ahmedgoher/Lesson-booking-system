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
            // تحميل القوائم ورقم الواتساب
            await LoadViewDataAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> booking(Student booking)
        {
            // 🟢 1. إعادة تحميل القوائم ورقم الواتساب حتى لا تفرغ عند وجود خطأ
            await LoadViewDataAsync();

            // 🟢 2. التحقق من القوائم المنسدلة
            if (booking.ClassId == 0)
                ModelState.AddModelError("ClassId", "يرجى اختيار الصف الدراسي");

            if (booking.PlaceId == 0)
                ModelState.AddModelError("PlaceId", "يرجى اختيار مكان الدرس");

            if (booking.SlotId == 0)
                ModelState.AddModelError("SlotId", "يرجى اختيار المجموعة");

            // 🟢 3. إذا كان نموذج البيانات يحتوي على أخطاء من البداية
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(booking.Password))
            {
                ViewBag.ErrorMessage = "برجاء استكمال جميع البيانات وإدخال كلمة المرور واختيار الصف، المكان، والمجموعة بشكل صحيح.";
                return View(booking);
            }

            // 🟢 4. محاولة إنشاء الحساب في السيرفس
            var result = await _bookingService.CreateBookingAsync(booking);

            if (result)
            {
                ViewBag.SuccessMessage = "تم إنشاء حسابك وتسجيل بيانات الحجز بنجاح!";
                return View(new Student()); // تفريغ النموذج بعد النجاح
            }

            // 🔴 5. إذا فشل الحفظ (غالباً بسبب تكرار رقم الهاتف)
            ModelState.AddModelError("StdPhone", "رقم الهاتف هذا مسجل بالفعل لطالب آخر!");
            ViewBag.ErrorMessage = "حدث خطأ أثناء تسجيل الحساب، قد يكون رقم الهاتف مسجلاً بالفعل.";

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
                    : s.Group_Name
            }).ToList();

            return Json(result);
        }

        // 🛠️ دالة مساعدة لتجهيز بيانات الـ View بدون تكرار الكود
        private async Task LoadViewDataAsync()
        {
            var admins = _admin.GetAll();
            ViewBag.number = admins.FirstOrDefault()?.PhoneNumber ?? "";

            ViewBag.Classes = await _classService.GetAllClassesAsync();
            ViewBag.Places = await _placeService.GetAllPlacesAsync();
        }
    }
}