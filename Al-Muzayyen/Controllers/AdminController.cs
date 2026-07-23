using Al_Muzayyen.Models;
using Al_Muzayyen.Repositories;
using Al_Muzayyen.Services;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Al_Muzayyen.Controllers;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using System.Security.Claims;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
    {
        private readonly IGenericService<Class> _classService;
        private readonly IGenericService<Admin> _AdminService;
        private readonly IGenericService<Student> _bookingService;
        private readonly IGenericService<Available_slot> _availableSlotService;
        private readonly IGenericService<Slot_time> _slotTimeService;
    private readonly IGenericService<Place> _placeServiceGeneric;
        private readonly IGenericService<Video> _videoService;
        private readonly IPlaceService _placeService;
        private readonly IBookingRepo bookingRepo;
        private readonly IGroupRepo groupRepo;
        private readonly IConfiguration _configuration;
    private readonly IClassService _ClassService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly CloudinaryService _cloudinaryService;
    private readonly IExamService _examService;
    public AdminController

            (IGenericService<Slot_time> slotTimeService,
        IClassService ClassService,
        CloudinaryService cloudinaryService, IWebHostEnvironment webHostEnvironment,
            IGenericService<Admin> adminService,
            IGenericService<Class> classService,
            IGenericService<Student> bookingService,
            IPlaceService placeService,
            IGenericService<Video> videoService,
            IGenericService<Place> genericServiceGeneric,
            IGenericService<Available_slot> availableSlotService,
            IBookingRepo bookingRepo,
            IGroupRepo groupRepo,
            IConfiguration configuration, IExamService examService)
        {
        _slotTimeService = slotTimeService;
        _cloudinaryService = cloudinaryService;
        _webHostEnvironment = webHostEnvironment;
        _AdminService = adminService;
            _classService = classService;
            _bookingService = bookingService;
            _placeService = placeService;
            _availableSlotService = availableSlotService;
            _placeServiceGeneric = genericServiceGeneric;
            _videoService = videoService;
            this.bookingRepo = bookingRepo;
            this.groupRepo = groupRepo;
            _configuration = configuration;
        _examService = examService;
        _ClassService = ClassService;
    }   
        public IActionResult Index()
        {
            var model = new IndexVMAdmin
            {
                StudentsCount = _bookingService.GetAll().Count(),
                GroupsCount = _availableSlotService.GetAll().Count(),
                ClassesCount = _classService.GetAll().Count(),
                Admin = _AdminService.GetAll().FirstOrDefault()

            };
            return View(model);
        }

        public async Task<IActionResult> Students(int? placeId, int? classId, int? groupId)
        {
            var stds = await bookingRepo.GetEnteredStudents();
            var query = stds.AsQueryable();

            if (placeId.HasValue)
            {
                query = query.Where(s => s.Id == placeId.Value);
            }

            if (classId.HasValue)
            {
                query = query.Where(s => s.Id == classId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(s => s.SlotId == groupId.Value);
            }

            ViewBag.Places = _placeServiceGeneric.GetAll();
            ViewBag.Classes = _classService.GetAll();
            ViewBag.Groups = _availableSlotService.GetAll();

            ViewBag.SelectedPlace = placeId;
            ViewBag.SelectedClass = classId;
            ViewBag.SelectedGroup = groupId;

            return View(query.ToList());
        }

    [HttpGet]
    public async Task<IActionResult> ExportStudents(int? placeId, int? classId, int? groupId)
    {
        var stds = await bookingRepo.GetEnteredStudents();
        var query = stds.AsQueryable();

        if (placeId.HasValue)
            query = query.Where(s => s.Id == placeId.Value);

        if (classId.HasValue)
            query = query.Where(s => s.Id == classId.Value);

        if (groupId.HasValue)
            query = query.Where(s => s.SlotId == groupId.Value);

        var students = query.ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("الطلاب المسجلين");

        // اتجاه الشيت يبقى من اليمين لليسار
        worksheet.RightToLeft = true;

        // رؤوس الأعمدة
        worksheet.Cell(1, 1).Value = "الاسم";
        worksheet.Cell(1, 2).Value = "الهاتف";
        worksheet.Cell(1, 3).Value = "الصف";
        worksheet.Cell(1, 4).Value = "المكان";
        worksheet.Cell(1, 5).Value = "المجموعة";
        worksheet.Cell(1, 6).Value = "الوقت";

        // تنسيق صف العناوين
        var headerRow = worksheet.Range(1, 1, 1, 6);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#c9a227");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // البيانات
        int row = 2;
        foreach (var std in students)
        {
            worksheet.Cell(row, 1).Value = std.Name;
            worksheet.Cell(row, 2).Value = std.StdPhone;
            worksheet.Cell(row, 3).Value = std.Class?.Name;
            worksheet.Cell(row, 4).Value = std.Place?.Name;
            worksheet.Cell(row, 5).Value = std.AvailableSlot?.Group_Name;
            worksheet.Cell(row, 6).Value = std.CreatedAt.ToString("hh:mm tt");
            row++;
        }

        // تعديل عرض الأعمدة تلقائي حسب المحتوى
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        string fileName = $"الطلاب_المسجلين_{DateTime.Now:yyyy-MM-dd}.xlsx";
        
        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }
    //public IActionResult FetchStudents()
    //{

    //    return View();
    //}

    [HttpPost]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);

        if (booking != null)
        {
            try
            {
                _bookingService.Delete(booking);
                await _bookingService.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم إلغاء الحجز بنجاح!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء إلغاء الحجز.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "الحجز غير موجود.";
        }

        return RedirectToAction(nameof(Students));
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
        //[HttpGet]
        //public IActionResult Create(int? Number_Of_day)
        //{
        //    ViewBag.Places = _placeServiceGeneric.GetAll();
        //    ViewBag.Classes = _classService.GetAll();

        //    ViewBag.Number_Of_day = Number_Of_day ?? 1;

        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create(Available_slot group)
        //{
        //    ModelState.Remove("SlotTimes.AvailableSlot");
        //    ModelState.Remove("SlotTimes.AvailableSlotId");
        //    if (ModelState.IsValid)
        //    {
        //        _availableSlotService.Add(group);
        //        await _availableSlotService.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "تم إضافة المجموعة بنجاح!";
        //        return RedirectToAction(nameof(Groups));
        //    }

        //    ViewBag.Places = _placeServiceGeneric.GetAll();
        //    ViewBag.Classes = _classService.GetAll();
        //    return View(group);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] GroupActionVM dto)
        {
            if (dto == null)
            {
                return Json(new { success = false, message = "فشل في قراءة البيانات المرسلة (Null Payload)" });
            }

            if (string.IsNullOrEmpty(dto.Group_Name))
            {
                return Json(new { success = false, message = "اسم المجموعة مطلوب!" });
            }

            var group = new Available_slot
            {
                Group_Name = dto.Group_Name,
                PlaceId = dto.PlaceId,
                ClassId = dto.ClassId,
                Number_Of_day = dto.Number_Of_day,
                State = "Active",
                SlotTimes = dto.SlotTimes.Select(s => new Slot_time
                {
                    Day = s.Day,
                    Time = DateTime.Parse(s.Time)
                }).ToList()
            };
                _availableSlotService.Add(group);
                await _availableSlotService.SaveChangesAsync();
                return Json(new { success = true, message = "تم إضافة المجموعة بنجاح!" });
        }
        [HttpGet]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var groups = await groupRepo.GetAllGroupsWithRelations();
            var group = groups.FirstOrDefault(g => g.Id == id);
            if(group == null) return NotFound();
            var result = new
            {
                id = group.Id,
                name = group.Group_Name,
                placeId = group.PlaceId,
                classId = group.ClassId,
                slots = group.SlotTimes.Select(s => new {day=s.Day,time=s.Time})
            };
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditAjax([FromBody] GroupActionVM dto)
        {
            if (dto == null || dto.Id == 0)
            {
                return Json(new { success = false, message = "بيانات التعديل غير صحيحة (Null Payload)" });
            }

            var group = new Available_slot
            {
                Id = dto.Id,
                Group_Name = dto.Group_Name,
                PlaceId = dto.PlaceId,
                ClassId = dto.ClassId,
                Number_Of_day = dto.Number_Of_day,
                SlotTimes = dto.SlotTimes?.Select(s => new Slot_time
                {
                    Day = s.Day,
                    Time = DateTime.Parse(s.Time)
                }).ToList()
            };

            try
            {
                await groupRepo.UpdateGroupWithSlots(group);
                return Json(new { success = true, message = "تم تعديل المجموعة بنجاح!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء تعديل المجموعة!" });
            }
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
        try
        {
            var item = await _classService.GetByIdAsync(id);

            if (item == null)
            {
                TempData["DeleteClassError"] = "الصف غير موجود.";
                return RedirectToAction(nameof(Classes));
            }
            var slots = _availableSlotService.GetAll()
                                             .Where(x => x.ClassId == id)
                                             .ToList();

            foreach (var slot in slots)
            {
                var times = _slotTimeService.GetAll()
                                            .Where(x => x.SlotID == slot.Id)
                                            .ToList();

                foreach (var time in times)
                {
                    _slotTimeService.Delete(time);
                }

                _slotTimeService.SaveChanges();

                _availableSlotService.Delete(slot);
            }

            _availableSlotService.SaveChanges();

            _classService.Delete(item);
            _classService.SaveChanges();

            TempData["DeleteClassSuccess"] = "تم حذف الصف بنجاح.";
        }
        catch
        {
            TempData["DeleteClassError"] = "تعذر حذف الصف، لأنه مرتبط ببيانات أخرى.";
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
            video.URL = GetMediaUrl(video.URL);

                _videoService.Update(video);
                _videoService.SaveChanges();

                TempData["SuccessVideo"] = "تم تعديل الفيديو بنجاح.";
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

           video.URL=GetMediaUrl(video.URL);
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
        // 1. عرض صفحة تعديل الحساب
        //public IActionResult ChangeProfile()
        //{
        //    // قراءة البيانات الحالية وعرضها في الصفحة
        //    ViewBag.CurrentUsername = _configuration["AdminSettings:Username"];
        //    return View();
        //}

    // 2. استقبال البيانات الجديدة وحفظها في الـ appsettings.json
    //[HttpPost]
    //public IActionResult ChangeProfile(string newUsername, string newPassword)
    //{
    //    if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword))
    //    {
    //        TempData["Error"] = "اسم المستخدم وكلمة المرور مطلوبة.";
    //        return View();
    //    }

    //    try
    //    {
    //        // مسار ملف appsettings.json الحقيقي على السيرفر
    //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
    //        var json = System.IO.File.ReadAllText(filePath);

    //        // تعديل القيم ديناميكياً داخل نص الـ JSON
    //        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
    //        jsonObj["AdminSettings"]["Username"] = newUsername;
    //        jsonObj["AdminSettings"]["Password"] = newPassword;

    //        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
    //        System.IO.File.WriteAllText(filePath, output);

    //        TempData["Success"] = "تم تحديث بيانات الحساب بنجاح! يرجى تسجيل الدخول مجدداً بالبيانات الجديدة.";

    //        // طرد الآدمن لصفحة اللوجن عشان يدخل بالبيانات الجديدة لتأكيد الحفظ
    //        return RedirectToAction("Logout", "Account");
    //    }
    //    catch (Exception)
    //    {
    //        TempData["Error"] = "حدث خطأ أثناء حفظ البيانات الجديدة.";
    //        return View();
    //    }
    //}




    // 🟢 1. عرض صفحة تعديل البيانات (GET)
    [HttpGet]
    public IActionResult ChangeProfile()
    {
        // جلب ID الآدمن الحالي من الكوكيز
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Admin admin = null;
        if (int.TryParse(adminIdClaim, out int adminId))
        {
            admin = _AdminService.GetAll().FirstOrDefault(a => a.Id == adminId);
        }

        // في حال عدم العثور عليه بـ ID الكوكيز، نأخذ أول أدمن متاح
        if (admin == null)
        {
            admin = _AdminService.GetAll().FirstOrDefault();
        }

        if (admin == null)
        {
            return RedirectToAction("login2", "Account");
        }

        // تمرير البيانات الحالية للـ View
        ViewBag.CurrentUsername = admin.Name;
        ViewBag.CurrentPhone = admin.PhoneNumber;

        return View();
    }

    // 🟢 2. حفظ البيانات الجديدة في قاعدة البيانات (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeProfile(string newUsername, string newPhone, string newPassword)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Admin admin = null;
        if (int.TryParse(adminIdClaim, out int adminId))
        {
            admin = _AdminService.GetAll().FirstOrDefault(a => a.Id == adminId);
        }

        if (admin == null)
        {
            admin = _AdminService.GetAll().FirstOrDefault();
        }

        if (admin == null)
        {
            TempData["Error"] = "لم يتم العثور على حساب الآدمن!";
            return RedirectToAction(nameof(ChangeProfile));
        }

        // التحقق من أن الحقول ليست فارغة
        if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
        {
            TempData["Error"] = "برجاء ملء جميع الحقول المطلوبة!";
            return RedirectToAction(nameof(ChangeProfile));
        }

        // تحديث البيانات
        admin.Name = newUsername;
        admin.Password = newPassword;

        if (!string.IsNullOrWhiteSpace(newPhone))
        {
            admin.PhoneNumber = newPhone;
        }

        // حفظ التعديلات باستخدام _AdminService
        _AdminService.Update(admin);
        _AdminService.SaveChanges();

        TempData["Success"] = "تم تحديث بيانات الحساب بنجاح!";
        return RedirectToAction(nameof(ChangeProfile));
    }










    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileImage(IFormFile imageFile)
    {
        // 1. التأكد من أن المستخدم اختار صورة بالفعل
        if (imageFile == null || imageFile.Length == 0)
        {
            ModelState.AddModelError("", "برجاء اختيار ملف صورة صحيح.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            string imageUrl = "";

            // 2. الرفع باستخدام الـ Service الخاصة بك تماماً مثل الـ Speciality
            imageUrl = await _cloudinaryService.UploadImageAsync(imageFile);

            // 3. جلب الأدمن الحالي من قاعدة البيانات
            var admin = _AdminService.GetAll().FirstOrDefault();

            if (admin == null)
            {
                // إذا كان الأدمن غير موجود، نقوم بإنشاء سجل جديد وحفظ رابط الصورة فيه
                var newAdmin = new Admin
                {
                    ImageUrl = imageUrl,
                    Name = "الأستاذ عبد الفتاح المزين", // قيم افتراضية حتى يقوم بتعديلها لاحقاً
                    PhoneNumber = "غير محدد"
                };
                _AdminService.Add(newAdmin);
            }
            else
            {
                // إذا كان موجوداً، نقوم بتحديث رابط الصورة فقط
                admin.ImageUrl = imageUrl;
            }

            // 4. حفظ التغييرات في قاعدة البيانات
            _AdminService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // معالجة الخطأ في حالة حدوث مشكلة أثناء الرفع
            return RedirectToAction(nameof(Index));
        }
    }
    //public IActionResult UpdateProfileData(Admin updatedModel)
    //    {
    //        // نقوم بفحص الحقول الأساسية فقط المطلوبة في الـ Popup
    //        if (string.IsNullOrEmpty(updatedModel.Name) || string.IsNullOrEmpty(updatedModel.PhoneNumber))
    //        {
    //            ModelState.AddModelError("", "برجاء ملء جميع الحقول المطلوبة.");
    //            return RedirectToAction(nameof(Index));
    //        }

    //        try
    //        {
    //            // جلب الأدمن الحالي من قاعدة البيانات
    //            var admin = _AdminService.GetAll().FirstOrDefault();

    //            if (admin == null)
    //            {
    //                // 1. في حالة الـ null: نقوم بإنشاء سجل جديد تماماً
    //                var newAdmin = new Admin
    //                {
    //                    Name = updatedModel.Name,
    //                    PhoneNumber = updatedModel.PhoneNumber
    //                    // يمكنك إضافة كلمة مرور افتراضية هنا لو أحببت:
    //                    // Password = "..." 
    //                };

    //                // تأكد من أن الـ Service بتاعتك تدعم دالة الإضافة مثل Add أو Insert
    //                _AdminService.Add(newAdmin);
    //            }
    //            else
    //            {
    //                // 2. في حالة وجود بيانات: نقوم بالتحديث الطبيعي
    //                admin.Name = updatedModel.Name;
    //                admin.PhoneNumber = updatedModel.PhoneNumber;
    //                // admin.Password = updatedModel.Password;
    //            }

    //            // حفظ التغييرات في قاعدة البيانات (سواء كانت إضافة أو تحديث)
    //            _AdminService.SaveChanges();

    //            // إعادة التوجيه لصفحة الـ Dashboard لرؤية البيانات الجديدة
    //            return RedirectToAction(nameof(Index));
    //        }
    //        catch (Exception ex)
    //        {
    //            // يمكنك معالجة الخطأ أو تسجيله هنا (Logging)
    //            return RedirectToAction(nameof(Index));
    //        }
    //    }




    private string GetMediaUrl(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";

            // ==========================
            // Google Drive
            // ==========================
            if (url.Contains("drive.google.com/file/d/"))
            {
                var id = url.Split("/file/d/")[1].Split('/')[0];

                // لو فيديو
                return $"https://drive.google.com/file/d/{id}/preview";

                // لو صورة استخدم السطر ده بدلاً من اللي فوق
                // return $"https://drive.google.com/uc?export=view&id={id}";
            }

            // ==========================
            // YouTube (youtu.be)
            // ==========================
            if (url.Contains("youtu.be/"))
            {
                var id = url.Split("youtu.be/")[1].Split('?')[0];
                return $"https://www.youtube.com/embed/{id}";
            }

            // ==========================
            // YouTube (watch?v=)
            // ==========================
            if (url.Contains("youtube.com/watch?v="))
            {
                var id = url.Split("watch?v=")[1].Split('&')[0];
                return $"https://www.youtube.com/embed/{id}";
            }

            // ==========================
            // YouTube Shorts
            // ==========================
            if (url.Contains("youtube.com/shorts/"))
            {
                var id = url.Split("shorts/")[1].Split('?')[0];
                return $"https://www.youtube.com/embed/{id}";
            }

            // أي رابط آخر
            return url;
        }
        catch (Exception)
        {
            // لو حصل أي خطأ، رجع الرابط كما هو
            return url;
        }
    }

        // 2. الأكشن الخاص بتعديل البيانات الأساسية (الاسم، الهاتف، الباسورد)
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public IActionResult UpdateProfileData(Admin updatedModel)
        {
            // نقوم بفحص الحقول الأساسية فقط المطلوبة في الـ Popup
            if (string.IsNullOrEmpty(updatedModel.Name) || string.IsNullOrEmpty(updatedModel.PhoneNumber))
            {
                ModelState.AddModelError("", "برجاء ملء جميع الحقول المطلوبة.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // جلب الأدمن الحالي من قاعدة البيانات
                var admin = _AdminService.GetAll().FirstOrDefault();

                if (admin == null)
                {
                    // 1. في حالة الـ null: نقوم بإنشاء سجل جديد تماماً
                    var newAdmin = new Admin
                    {
                        Name = updatedModel.Name,
                        PhoneNumber = updatedModel.PhoneNumber
                        // يمكنك إضافة كلمة مرور افتراضية هنا لو أحببت:
                        // Password = "..." 
                    };

                    // تأكد من أن الـ Service بتاعتك تدعم دالة الإضافة مثل Add أو Insert
                    _AdminService.Add(newAdmin);
                }
                else
                {
                    // 2. في حالة وجود بيانات: نقوم بالتحديث الطبيعي
                    admin.Name = updatedModel.Name;
                    admin.PhoneNumber = updatedModel.PhoneNumber;
                    // admin.Password = updatedModel.Password;
                }

                // حفظ التغييرات في قاعدة البيانات (سواء كانت إضافة أو تحديث)
                _AdminService.SaveChanges();

                // إعادة التوجيه لصفحة الـ Dashboard لرؤية البيانات الجديدة
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // يمكنك معالجة الخطأ أو تسجيله هنا (Logging)
                return RedirectToAction(nameof(Index));
            }

    }









    //===================================================================================================





    // 1. عرض صفحة الامتحانات وجلب الصفوف الدراسية ديناميكياً
    public async Task<IActionResult> Exams()
    {
        // جلب قائمة الامتحانات
        var examsList = await _examService.GetAllExamsViewModelsAsync();

        // جلب الصفوف من _classService المحقونة لديك (IGenericService<Class>)
        ViewBag.Classes = await _ClassService.GetAllClassesAsync();

        return View(examsList);
    }

    // 2. حفظ الامتحان (إضافة أو تعديل)
    [HttpPost]
    public async Task<IActionResult> SaveExam([FromBody] ExamViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return Json(new { success = false, message = "يرجى إدخال عنوان الامتحان" });
        }

        if (model.GradeId <= 0)
        {
            return Json(new { success = false, message = "يرجى اختيار الصف الدراسي" });
        }

        var examEntity = new Exam
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description, // 🟢 إضافة الوصف
            DurationMinutes = model.Duration > 0 ? model.Duration : 30,
            IsActive = model.Status == "Active",
            CreatedAt = model.Date != default ? model.Date : DateTime.Today,
            StartExamTime = model.OpenDate ?? DateTime.Now,
            EndExamTime = model.CloseDate ?? DateTime.Now.AddHours(2),
            ClassId = model.GradeId,

            // 🟢 ربط وحفظ كافة الخصائص والإعدادات الجديدة
            TotalMarks = model.TotalMarks,
            PassingMarks = model.PassingMarks,
            MaxAttempts = model.MaxAttempts > 0 ? model.MaxAttempts : 1,
            RandomQuestions = model.RandomQuestions,
            ShuffleAnswers = model.ShuffleAnswers,
            AllowReview = model.AllowReview,
            ShowResult = model.ShowResult
        };

        if (model.Id > 0)
        {
            await _examService.UpdateExamAsync(examEntity);
            return Json(new { success = true, message = "تم تعديل الامتحان بنجاح!" });
        }
        else
        {
            await _examService.CreateExamAsync(examEntity);
            return Json(new { success = true, message = "تم إضافة الامتحان بنجاح!" });
        }
    }
    public IActionResult QExams()
    {
        return View();
    }



}

