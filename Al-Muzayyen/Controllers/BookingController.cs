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
            // 1. التحقق من صحة البيانات ومن إدخال كلمة المرور واختيار الصف والمكان والمجموعة
            if (!ModelState.IsValid
                || string.IsNullOrWhiteSpace(booking.Password)
                || booking.ClassId == 0
                || booking.PlaceId == 0
                || booking.SlotId == 0)
            {
                ViewBag.ErrorMessage = "برجاء استكمال جميع البيانات وإدخال كلمة المرور واختيار الصف، المكان، والمجموعة بشكل صحيح.";

                ViewBag.Classes = await _classService.GetAllClassesAsync();
                ViewBag.Places = await _placeService.GetAllPlacesAsync();
                return View(booking);
            }

            // 2. إنشاء الحساب وتخزين بيانات الطالب في الداتا بيز بواسطة السيرفس
            var result = await _bookingService.CreateBookingAsync(booking);

            if (result)
            {
                ViewBag.SuccessMessage = " تم إنشاء حسابك وتسجيل بيانات الحجز بنجاح!";

                ViewBag.Classes = await _classService.GetAllClassesAsync();
                ViewBag.Places = await _placeService.GetAllPlacesAsync();

                return View(new Student()); // إعادة كائن جديد فارغ لتفريغ النماذج
            }

            ViewBag.ErrorMessage = "حدث خطأ أثناء تسجيل الحساب، قد يكون رقم الهاتف مسجلاً بالفعل.";
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