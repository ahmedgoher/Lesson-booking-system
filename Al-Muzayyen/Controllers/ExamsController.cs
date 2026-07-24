using Microsoft.AspNetCore.Mvc;

namespace Al_Muzayyen.Controllers
{
    public class ExamsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowExam()
        {
            return View();
        }
        public async Task<IActionResult> PaperExam()
        {
            return View();
        }
      
    }
}
