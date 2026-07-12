using Microsoft.AspNetCore.Mvc;

namespace Al_Muzayyen.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult booking()
        {
            return View();
        }
    }
}
