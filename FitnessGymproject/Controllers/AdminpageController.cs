using FitnessGymproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessGymproject.Controllers
{
    public class AdminpageController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminpageController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var registeredMembersCount = await _context.Members.CountAsync();
            var previousMonthCount = await _context.Members
                .CountAsync(m => m.CreatedAt >= DateTime.Now.AddDays(-30));

            var growthRate = registeredMembersCount > 0
                ? Math.Round((previousMonthCount / (double)registeredMembersCount) * 100, 2)
                : 0;
            var activeSubscriptionsCount = await _context.Subscriptions.CountAsync(s => s.Status == "Active");
            var totalRevenue = await _context.Payments.SumAsync(p => p.Amount) ?? 0;

            var loggedInAdminId = HttpContext.Session.GetString("LoggedInAdminId");

            if (string.IsNullOrEmpty(loggedInAdminId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal adminId = Convert.ToDecimal(loggedInAdminId);
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            ViewData["RegisteredMembersCount"] = registeredMembersCount;
            ViewData["ClientsGrowthRate"] = growthRate;
            ViewData["ActiveSubscriptionsCount"] = activeSubscriptionsCount;
            ViewData["TotalRevenue"] = totalRevenue;
            ViewData["AdminName"] = admin.FullName;
            ViewData["LoggedInAdminId"] = admin.AdminId;

            return View();
        }

        public async Task<IActionResult> AdminProfile()
        {
            var loggedInAdminId = HttpContext.Session.GetString("LoggedInAdminId");

            if (string.IsNullOrEmpty(loggedInAdminId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal adminId = Convert.ToDecimal(loggedInAdminId);
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == adminId);

            if (admin == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("LoggedInAdminImage", admin.Imageprofileurl);

            ViewData["AdminId"] = admin.AdminId;
            ViewData["AdminName"] = admin.FullName;
            ViewData["AdminEmail"] = admin.Email;
            ViewData["AdminGender"] = admin.Gender;
            ViewData["AdminImage"] = admin.Imageprofileurl;

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,FullName,Email,Password,CreatedAt,UpdatedAt,Gender,Imageprofileurl,ImageFile")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                if (admin.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(admin.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await admin.ImageFile.CopyToAsync(fileStream);
                    }

                    admin.Imageprofileurl = "/images/" + fileName;
                }

                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminProfile));
            }
            return View(admin);
        }

        public async Task<IActionResult> Edit()
        {
            decimal loggedInAdminId = Convert.ToDecimal(HttpContext.Session.GetString("LoggedInAdminId"));
            var admin = await _context.Admins.FindAsync(loggedInAdminId);

            if (admin == null)
            {
                return NotFound();
            }

            ViewData["AdminName"] = HttpContext.Session.GetString("LoggedInAdminName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("LoggedInAdminEmail");
            ViewData["AdminGender"] = HttpContext.Session.GetString("LoggedInAdminGender");
            ViewData["AdminId"] = HttpContext.Session.GetString("LoggedInAdminId");

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("AdminId,FullName,Email,Password,CreatedAt,UpdatedAt,Gender,Imageprofileurl,ImageFile")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var loggedInAdminId = HttpContext.Session.GetString("LoggedInAdminId");
                    if (loggedInAdminId != admin.AdminId.ToString())
                    {
                        return Unauthorized();
                    }

                    if (admin.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;

                        if (!string.IsNullOrEmpty(admin.Imageprofileurl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, admin.Imageprofileurl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(admin.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath, "images", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await admin.ImageFile.CopyToAsync(fileStream);
                        }

                        admin.Imageprofileurl = "/images/" + fileName;
                    }

                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                HttpContext.Session.SetString("LoggedInAdminName", admin.FullName);
                HttpContext.Session.SetString("LoggedInAdminEmail", admin.Email);
                HttpContext.Session.SetString("LoggedInAdminGender", admin.Gender);

                return RedirectToAction(nameof(AdminProfile));
            }
            return View(admin);
        }

        public async Task<IActionResult> MonthlyReport(string searchTerm = "")
        {
            var subscriptionsQuery = _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Payment)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                subscriptionsQuery = subscriptionsQuery.Where(s => s.Member.FullName.Contains(searchTerm));
            }

            var monthlySubscriptions = await subscriptionsQuery
                .Where(s => s.StartDate.HasValue && s.StartDate.Value.Month == DateTime.Now.Month)
                .ToListAsync();

            decimal monthlyRevenue = monthlySubscriptions
                .Sum(s => (s.TotalPayment ?? 0) - (s.Payment?.Amount ?? 0));

            var model = new
            {
                MonthlySubscriptions = monthlySubscriptions,
                MonthlyRevenue = monthlyRevenue
            };

            return View("MonthlyReport", model);
        }

        public async Task<IActionResult> AnnualReport(string searchTerm = "")
        {
            var subscriptionsQuery = _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Payment)
                .Include(s => s.WorkoutPlan)
                .Include(s => s.MembershipPlan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                subscriptionsQuery = subscriptionsQuery.Where(s => s.Member.FullName.Contains(searchTerm));
            }

            var annualSubscriptions = await subscriptionsQuery
                .Where(s => s.StartDate.HasValue && s.StartDate.Value.Year == DateTime.Now.Year)
                .ToListAsync();

            var annualRevenue = annualSubscriptions.Sum(s =>
            {
                decimal sellingPrice = s.Payment?.Amount ??
                                       s.MembershipPlan?.Price ??
                                       s.WorkoutPlan?.Price ?? 0;

                decimal quantitySold = s.MembershipPlan?.DurationDays ?? 1;

                return sellingPrice * quantitySold;
            });

            ViewData["AnnualSubscriptions"] = annualSubscriptions;
            ViewData["AnnualRevenue"] = annualRevenue;

            return View("AnnualReport");
        }

        public IActionResult MonthlyChart()
        {
            var subscriptions = _context.Subscriptions
                .Include(s => s.Payment)
                .ToList();

            var monthlyRevenueData = new decimal[12];

            foreach (var subscription in subscriptions)
            {
                if (subscription.StartDate.HasValue)
                {
                    var monthIndex = subscription.StartDate.Value.Month - 1;
                    decimal totalSales = subscription.TotalPayment ?? 0;
                    decimal discountsOrReturns = subscription.Payment?.Amount ?? 0;

                    decimal monthlyRevenue = totalSales - discountsOrReturns;
                    monthlyRevenueData[monthIndex] += monthlyRevenue;
                }
            }

            decimal totalMonthlyRevenue = monthlyRevenueData.Sum();

            ViewData["MonthlyRevenueData"] = monthlyRevenueData;
            ViewData["TotalMonthlyRevenue"] = totalMonthlyRevenue;

            return View();
        }

        public IActionResult AnnualChart()
        {
            var subscriptions = _context.Subscriptions
                .Include(s => s.Payment)
                .Include(s => s.MembershipPlan)
                .Include(s => s.WorkoutPlan)
                .ToList();

            var annualRevenueData = new Dictionary<int, decimal>();

            foreach (var subscription in subscriptions)
            {
                if (subscription.StartDate.HasValue)
                {
                    var year = subscription.StartDate.Value.Year;
                    decimal sellingPrice = subscription.Payment?.Amount ??
                                           subscription.MembershipPlan?.Price ??
                                           subscription.WorkoutPlan?.Price ?? 0;

                    decimal quantitySold = subscription.MembershipPlan?.DurationDays ?? 1;

                    decimal revenue = sellingPrice * quantitySold;

                    if (annualRevenueData.ContainsKey(year))
                    {
                        annualRevenueData[year] += revenue;
                    }
                    else
                    {
                        annualRevenueData[year] = revenue;
                    }
                }
            }

            decimal totalAnnualRevenue = annualRevenueData.Values.Sum();

            ViewData["AnnualRevenueData"] = annualRevenueData;
            ViewData["TotalAnnualRevenue"] = totalAnnualRevenue;

            return View();
        }

        public async Task<IActionResult> Details()
        {
            var loggedInAdminId = HttpContext.Session.GetString("LoggedInAdminId");

            if (string.IsNullOrEmpty(loggedInAdminId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            if (!decimal.TryParse(loggedInAdminId, out var adminId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }


        // GET: Admin/Delete/{id}
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            var loggedInAdminId = HttpContext.Session.GetString("LoggedInAdminId");
            if (string.IsNullOrEmpty(loggedInAdminId))
            {
                return Unauthorized();
            }

            return View(admin); 
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            var loggedInAdminId = HttpContext.Session.GetString("LoggedInAdminId");

            if (string.IsNullOrEmpty(loggedInAdminId) || Convert.ToDecimal(loggedInAdminId) != id)
            {
                return Unauthorized(); 
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(); 
            }

            _context.Admins.Remove(admin); 
            await _context.SaveChangesAsync();

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "LoginAndRegister");
        }

        private bool AdminExists(decimal id)
        {
            return _context.Admins?.Any(e => e.AdminId == id) ?? false;
        }
    }
}
