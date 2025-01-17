using System.Diagnostics;
using FitnessGymproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessGymproject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;
        public HomeController(ILogger<HomeController> logger , ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var homePageContents = await _context.Homepages.ToListAsync(); // Fetch data from database

            // Check if homePageContents is null or empty, to handle any data retrieval issues
            if (homePageContents == null)
            {
                homePageContents = new List<Homepage>(); // Prevent null reference by providing an empty list
            }

            return View(homePageContents); // Pass the data to the view
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
