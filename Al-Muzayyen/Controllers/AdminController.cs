using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;
namespace Al_Muzayyen.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenericService<Class> _classService;
        private readonly IGenericService<Booking> _bookingService;
        private readonly IGenericService<Available_slot> _availableSlotService;
        private readonly IGenericService<Place> _placeServiceGeneric;
        private readonly IGenericService<Video> _videoService;
        private readonly IPlaceService _placeService;
        private readonly IBookingRepo bookingRepo;
        private readonly IGroupRepo groupRepo;

        public AdminController
            (IGenericService<Class> classService,
            IGenericService<Booking> bookingService,
            IPlaceService placeService,
            IGenericService<Video> videoService,
            IGenericService<Place> genericServiceGeneric,
            IGenericService<Available_slot> availableSlotService,
            IBookingRepo bookingRepo,
            IGroupRepo groupRepo)
        {
            _classService = classService;
            _bookingService = bookingService;
            _placeService = placeService;
            _availableSlotService = availableSlotService;
            _placeServiceGeneric = genericServiceGeneric;
            _videoService = videoService;
            this.bookingRepo = bookingRepo;
            this.groupRepo = groupRepo;

        }   
        public IActionResult Index()
        {
            var model = new IndexVMAdmin
            {
                StudentsCount = _bookingService.GetAll().Count(),
                GroupsCount = _availableSlotService.GetAll().Count(),
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

            if (classId.HasValue)
            {
                query = query.Where(g => g.ClassId == classId.Value);
            }
            ViewBag.Places = _placeServiceGeneric.GetAll();
            ViewBag.Classes = _classService.GetAll();

            ViewBag.SelectedPlace = placeId;
            ViewBag.SelectedClass = classId;
            ViewBag.SelectedStatus = status;

            return View(query.ToList());
        }
        [HttpGet]
        public IActionResult Create(int? Number_Of_day)
        {
            ViewBag.Places = _placeServiceGeneric.GetAll();
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

            ViewBag.Places = _placeServiceGeneric.GetAll();
            ViewBag.Classes = _classService.GetAll();
            return View(group);
        }
        public IActionResult Classes()
        {
            var classes = _classService.GetAll();
            return View(classes);
        }

        [HttpPost]
        public async Task<IActionResult> AddClass(Class model)
        {
            await _classService.AddAsync(model);
            _classService.SaveChanges();

            return RedirectToAction(nameof(Classes));
        }

        [HttpPost]
        public async Task<IActionResult> EditClass(Class model)
        {
            var item = await _classService.GetByIdAsync(model.Id);

            if (item != null)
            {
                item.Name = model.Name;

                _classService.Update(item);
                _classService.SaveChanges();
            }

            return RedirectToAction(nameof(Classes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var item = await _classService.GetByIdAsync(id);

            if (item != null)
            {
                _classService.Delete(item);
                _classService.SaveChanges();
            }

            return RedirectToAction(nameof(Classes));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int? Number_Of_day)
        {
            var groups = await groupRepo.GetAllGroupsWithRelations();
            var group = groups.FirstOrDefault(g => g.Id == id);

            if (group == null) return NotFound();

            ViewBag.Places = _placeServiceGeneric.GetAll();
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
                ViewBag.Places = _placeServiceGeneric.GetAll();
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

        [HttpPost]
        public async Task<IActionResult> AddVideo(Video video)
        {
            await _videoService.AddAsync(video);
            _videoService.SaveChanges();

            TempData["Success"] = "تمت إضافة الفيديو.";

            return RedirectToAction(nameof(Videos));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _videoService.GetByIdAsync(id);

            if (video != null)
            {
                _videoService.Delete(video);
                _videoService.SaveChanges();
            }

            TempData["Success"] = "تم حذف الفيديو.";

            return RedirectToAction(nameof(Videos));
        }

    }
}
