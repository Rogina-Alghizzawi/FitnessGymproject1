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


            // Pass data to the view
            ViewData["RegisteredMembersCount"] = registeredMembersCount;
            ViewData["ClientsGrowthRate"] = growthRate;
            ViewData["ActiveSubscriptionsCount"] = activeSubscriptionsCount;
            ViewData["TotalRevenue"] = totalRevenue;
            ViewData["AdminName"] = admin.FullName;
            ViewData["LoggedInAdminId"] = admin.AdminId;

            return View();
        }

        // GET: Adminpage/AdminProfile
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

        // POST: Adminpage/Create
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
    

// GET: Adminpage/Edit
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

        //public async Task<IActionResult> SubscriptionReport(string searchTerm = "")
        //{
        //    var subscriptionsQuery = _context.Subscriptions
        //        .Include(s => s.Member)
        //        .Include(s => s.Payment)
        //        .Include(s => s.Offer) // If you want to include offer details
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        subscriptionsQuery = subscriptionsQuery.Where(s => s.Member.FullName.Contains(searchTerm));
        //    }

        //    var monthlySubscriptions = await subscriptionsQuery
        //        .Where(s => s.StartDate.Value.Month == DateTime.Now.Month)
        //        .ToListAsync();

        //    var annualSubscriptions = await subscriptionsQuery
        //        .Where(s => s.StartDate.Value.Year == DateTime.Now.Year)
        //        .ToListAsync();

        //    var subscriptionReport = new
        //    {
        //        MonthlySubscriptions = monthlySubscriptions,
        //        AnnualSubscriptions = annualSubscriptions,
        //        TotalMonthlyRevenue = monthlySubscriptions.Sum(s => s.TotalPayment) ?? 0,
        //        TotalAnnualRevenue = annualSubscriptions.Sum(s => s.TotalPayment) ?? 0
        //    };

        //    // Add any other logic here as needed

        //    // Prepare data for the chart
        //    ViewData["MonthlyRevenue"] = subscriptionReport.TotalMonthlyRevenue;
        //    ViewData["AnnualRevenue"] = subscriptionReport.TotalAnnualRevenue;

        //    // Pass the data to the view
        //    return View(subscriptionReport);
        //}


      

            // Monthly Subscription Report
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
                    .Where(s => s.StartDate.Value.Month == DateTime.Now.Month)
                    .ToListAsync();

                var monthlyRevenue = monthlySubscriptions.Sum(s => s.TotalPayment) ?? 0;

                var model = new
                {
                    MonthlySubscriptions = monthlySubscriptions,
                    MonthlyRevenue = monthlyRevenue
                };

                return View("MonthlyReport", model);
            }

            // Annual Subscription Report
            public async Task<IActionResult> AnnualReport(string searchTerm = "")
            {
                var subscriptionsQuery = _context.Subscriptions
                    .Include(s => s.Member)
                    .Include(s => s.Payment)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    subscriptionsQuery = subscriptionsQuery.Where(s => s.Member.FullName.Contains(searchTerm));
                }

                var annualSubscriptions = await subscriptionsQuery
                    .Where(s => s.StartDate.Value.Year == DateTime.Now.Year)
                    .ToListAsync();

                var annualRevenue = annualSubscriptions.Sum(s => s.TotalPayment) ?? 0;

                var model = new
                {
                    AnnualSubscriptions = annualSubscriptions,
                    AnnualRevenue = annualRevenue
                };

                return View("AnnualReport", model);
            }
        

        //public IActionResult ExportToCSV()
        //{
        //    var subscriptions = _context.Subscriptions
        //        .Include(s => s.Member)
        //        .Include(s => s.Payment)
        //        .ToList();

        //    var csv = "PlanName, MemberName, Status, TotalPayment, PaymentStatus\n";

        //    foreach (var subscription in subscriptions)
        //    {
        //        csv += $"{subscription.PlanName}, {subscription.Member.FullName}, {subscription.Status}, {subscription.TotalPayment}, {subscription.PaymentStatus}\n";
        //    }

        //    return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "subscriptions.csv");
        //}


        //public IActionResult ExportToExcel()
        //{
        //    var subscriptions = _context.Subscriptions
        //        .Include(s => s.Member)
        //        .Include(s => s.Payment)
        //        .ToList();

        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Subscriptions");
        //        worksheet.Cells[1, 1].Value = "Plan Name";
        //        worksheet.Cells[1, 2].Value = "Member Name";
        //        worksheet.Cells[1, 3].Value = "Status";
        //        worksheet.Cells[1, 4].Value = "Total Payment";
        //        worksheet.Cells[1, 5].Value = "Payment Status";

        //        var row = 2;
        //        foreach (var subscription in subscriptions)
        //        {
        //            worksheet.Cells[row, 1].Value = subscription.PlanName;
        //            worksheet.Cells[row, 2].Value = subscription.Member.FullName;
        //            worksheet.Cells[row, 3].Value = subscription.Status;
        //            worksheet.Cells[row, 4].Value = subscription.TotalPayment;
        //            worksheet.Cells[row, 5].Value = subscription.PaymentStatus;
        //            row++;
        //        }

        //        var fileContent = package.GetAsByteArray();
        //        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "subscriptions.xlsx");
        //    }
        //}

        private bool AdminExists(decimal id)
        {
            return _context.Admins?.Any(e => e.AdminId == id) ?? false;
        }



    }
}

