using Al_Muzayyen.Models;
using Al_Muzayyen.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Controllers
{
    public class GroupManagementController : Controller
    {
        private readonly AppDbContext _context;

        public GroupManagementController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult GroupManagement(int id)
        {
            var group = _context.Available_Slots
                 .Include(s => s.Class)
                 .Include(s => s.Place)
                 .Include(s => s.Students)
                 .FirstOrDefault(s => s.Id == id);
            if (group == null)
            {
                return NotFound();
            }
            var allMaterials = _context.Materials
                .Where(m => m.SlotId == id)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            var viewModel = new GroupManagementViewModel
            {
                SlotId = group.Id,
                // تركيب الاسم باستخدام Properties الموجودة فعلياً
                GroupName = $"{group.Group_Name} ({(group.Class != null ? group.Class.Name : "")} - {(group.Place != null ? group.Place.Name : "")})",
                StudentCount = group.Students.Count,
                VideoCount = allMaterials.Count(m => m.Type == MaterialType.VideoLink),
                ExamCount = _context.Exams != null ? _context.Exams.Count(e => e.Id == id) : 0,
                Materials = allMaterials.Where(m => m.Type != MaterialType.VideoLink).ToList(),
                Videos = allMaterials.Where(m => m.Type == MaterialType.VideoLink).ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddMaterial(int slotId, string title, string url)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
            {
                var material = new Material
                {
                    SlotId = slotId,
                    Title = title,
                    Url = url,
                    Type = MaterialType.PDF,
                    CreatedAt = DateTime.Today
                };

                _context.Materials.Add(material);
                _context.SaveChanges();
            }
            return RedirectToAction("GroupManagement", new { id = slotId });
        }
        [HttpPost]
        public IActionResult DeleteMaterial(int id, int slotId)
        {
            var item = _context.Materials.Find(id);
            if (item != null)
            {
                _context.Materials.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("GroupManagement", new { id = slotId });
        }
        // إضافة فيديو جديد
        [HttpPost]
        public IActionResult AddVideo(int slotId, string title, string url)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
            {
                string embedUrl = url;

                // تحويل رابط YouTube العادي (watch?v=xxx)
                if (url.Contains("watch?v="))
                {
                    var videoId = url.Split("watch?v=")[1].Split('&')[0];
                    embedUrl = $"https://www.youtube.com/embed/{videoId}";
                }
                // تحويل رابط YouTube المختصر (youtu.be/xxx)
                else if (url.Contains("youtu.be/"))
                {
                    var videoId = url.Split("youtu.be/")[1].Split('?')[0];
                    embedUrl = $"https://www.youtube.com/embed/{videoId}";
                }

                var video = new Material
                {
                    SlotId = slotId,
                    Title = title,
                    Url = embedUrl,
                    Type = MaterialType.VideoLink,
                    CreatedAt = DateTime.Today
                };

                _context.Materials.Add(video);
                _context.SaveChanges();
            }

            return RedirectToAction("GroupManagement", new { id = slotId });
        }
    }
}
