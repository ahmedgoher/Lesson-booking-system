using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Al_Muzayyen.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenericService<Class> _classService;
        private readonly IGenericService<Booking> _bookingService;
        private readonly IGenericService<Place> _placeService;
        private readonly IGenericService<Available_slot> _availableSlotService;
        private readonly IGenericService<Slot_time> _slotTime;
        private readonly IGenericService<Video> _videoService;
        private readonly IBookingRepo bookingRepo;
        private readonly IGroupRepo groupRepo;



        public AdminController
            (IGenericService<Class> classService,
            IGenericService<Booking> bookingService,
            IGenericService<Place> placeService,
            IGenericService<Video> videoService,
            IGenericService<Slot_time> slotTime,
            IGenericService<Available_slot> availableSlotService,
            IBookingRepo bookingRepo,
            IGroupRepo groupRepo)
        {
            _classService = classService;
            _bookingService = bookingService;
            _placeService = placeService;
            _availableSlotService = availableSlotService;
            _videoService = videoService;
            _slotTime = slotTime;
            this.bookingRepo = bookingRepo;
            this.groupRepo = groupRepo;
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

        public async Task<IActionResult> Students()
        {
            var stds = await bookingRepo.GetEnteredStudents();
            return View(stds);
        }

        public async Task<IActionResult> Groups(int? placeId, int? classId, string status)
        {
            var groups = await groupRepo.GetAllGroupsWithRelations();
            var query = groups.AsQueryable();
            if (string.IsNullOrEmpty(status))
            {
                status = "Active";
            }
            if (status != "All") // لو اختار "الكل" هيعرض الكل، غير كده يفلتر حسب الاختيار
            {
                query = query.Where(g => g.State == status);
            }
            //  الفلترة بالمكان
            if (placeId.HasValue)
            {
                query = query.Where(g => g.PlaceId == placeId.Value);
            }

            //  الفلترة بالصف الدراسي
            if (classId.HasValue)
            {
                query = query.Where(g => g.ClassId == classId.Value);
            }
            ViewBag.Places = _placeService.GetAll();
            ViewBag.Classes = _classService.GetAll();

            // نحتفظ بالقيم الحالية للفلتر عشان تفضل مختارة بعد ما الصفحة تعمل Reload
            ViewBag.SelectedPlace = placeId;
            ViewBag.SelectedClass = classId;
            ViewBag.SelectedStatus = status;

            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create(int? Number_Of_day)
        {
            ViewBag.Places = _placeService.GetAll();
            ViewBag.Classes = _classService.GetAll();

            ViewBag.Number_Of_day = Number_Of_day ?? 1;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Available_slot group)
        {
            ModelState.Remove("SlotTimes.AvailableSlot");
            ModelState.Remove("SlotTimes.AvailableSlotId");
            if (ModelState.IsValid)
                {
                    _availableSlotService.Add(group);
                    await _availableSlotService.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم إضافة المجموعة بنجاح!";
                return RedirectToAction(nameof(Groups));
            }

            ViewBag.Places = _placeService.GetAll();
                ViewBag.Classes = _classService.GetAll();
                return View(group);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, int? Number_Of_day)
        {
            var groups = await groupRepo.GetAllGroupsWithRelations();
            var group = groups.FirstOrDefault(g => g.Id == id);

            if (group == null) return NotFound();

            ViewBag.Places = _placeService.GetAll();
            ViewBag.Classes = _classService.GetAll();

            ViewBag.Number_Of_day = Number_Of_day ?? group.SlotTimes.Count;

            return View(group);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Available_slot group)
        {
            ModelState.Remove("SlotTimes.AvailableSlot");
            ModelState.Remove("SlotTimes.AvailableSlotId");

            if (id != group.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Places = _placeService.GetAll();
                ViewBag.Classes = _classService.GetAll();
                ViewBag.Number_Of_day = group.SlotTimes?.Count ?? 1;
                return View(group);
            }

            await groupRepo.UpdateGroupWithSlots(group);

            TempData["SuccessMessage"] = "تم تعديل بيانات المجموعة بنجاح!";
            return RedirectToAction(nameof(Groups));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. هنجيب المجموعة من الـ Service عادي بدون ما نوجع دماغنا بالـ Relations
            var group = _availableSlotService.GetAll().FirstOrDefault(g => g.Id == id);

            if (group != null)
            {
                try
                {
                    group.State = "Closed";

                    _availableSlotService.Update(group);
                    await _availableSlotService.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم حذف (إغلاق) المجموعة بنجاح!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "حدث خطأ أثناء حذف المجموعة.";
                }
            }

            return RedirectToAction(nameof(Groups));
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
