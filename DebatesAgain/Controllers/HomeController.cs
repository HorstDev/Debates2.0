using DebatesAgain.Models;
using DebatesApp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DebatesAgain.Controllers
{
    public class HomeController : Controller
    {
        DebatesDataContext _db;

        public HomeController(DebatesDataContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}