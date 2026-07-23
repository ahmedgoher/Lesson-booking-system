using Microsoft.AspNetCore.Mvc;

namespace Al_Muzayyen.Controllers
{
    public class ExamsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
