using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using FitnessGymproject.Models;

namespace FitnessGymproject.Controllers
{
    public class MembersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MembersController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        public async Task<IActionResult> Index()
        {
            var members = await _context.Members.ToListAsync();
            return View(members);
        }

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FullName,Email,Password,PhoneNumber,Address,CreatedAt,UpdatedAt,Imageprofileurl,Gender,ImageFile")] Member member)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (member.ImageFile != null && member.ImageFile.Length > 0)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(member.ImageFile.FileName);
                        string filePath = Path.Combine(wwwRootPath, "Images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await member.ImageFile.CopyToAsync(fileStream);
                        }

                        member.Imageprofileurl = fileName;
                    }

                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the member profile: " + ex.Message);
                }
            }
            return View(member);
        }
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("MemberId,FullName,Email,Password,PhoneNumber,Address,CreatedAt,UpdatedAt,Imageprofileurl,Gender,ImageFile")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (member.ImageFile != null && member.ImageFile.Length > 0)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(member.ImageFile.FileName);
                        string filePath = Path.Combine(wwwRootPath, "Images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await member.ImageFile.CopyToAsync(fileStream);
                        }

                        if (!string.IsNullOrEmpty(member.Imageprofileurl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, "Images", member.Imageprofileurl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        member.Imageprofileurl = fileName;
                    }

                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(member);
        }


        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Profile()
        {
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(loggedInMemberId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal memberId = Convert.ToDecimal(loggedInMemberId);

            var member = await _context.Members.FirstOrDefaultAsync(t => t.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            ViewData["MemberId"] = member.MemberId;
            ViewData["FullName"] = member.FullName;
            ViewData["Email"] = member.Email;
            ViewData["PhoneNumber"] = member.PhoneNumber;
            ViewData["Address"] = member.Address;
            ViewData["CreatedAt"] = member.CreatedAt?.ToString("yyyy-MM-dd");
            ViewData["ImageProfileUrl"] = member.Imageprofileurl;
            ViewData["Gender"] = member.Gender;

            ViewData["WorkoutPlans"] = member.Workoutplans;

            return View(member);
        }

        public IActionResult ViewAvailablePlans()
        {
            // Get the memberId from the session
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

            if (loggedInMemberId == null)
            {
                return RedirectToAction("Login");  
            }

            decimal memberId = Convert.ToDecimal(loggedInMemberId);

            var availablePlans = _context.Workoutplans.Where(wp => wp.MemberId == null).ToList();

            ViewData["MemberId"] = memberId;

            return View(availablePlans);
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeToPlan(decimal workoutPlanId)
        {
            var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var memberId = decimal.Parse(memberIdString);

            var workoutPlan = await _context.Workoutplans.FindAsync(workoutPlanId);
            if (workoutPlan != null)
            {
                workoutPlan.MemberId = memberId;
                _context.Update(workoutPlan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewSubscribedPlans", "Members");
        }




        public IActionResult ViewSubscribedPlans()
        {
            string memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                return RedirectToAction("Login", "LoginAndRegister");

            }

            decimal memberId = Convert.ToDecimal(memberIdString);

            var workoutPlans = _context.Workoutplans
                                       .Where(wp => wp.MemberId == memberId)
                                       .ToList();

            if (workoutPlans == null || !workoutPlans.Any())
            {
                ViewBag.Message = "No workout plans assigned.";
                return View();
            }

            return View(workoutPlans);
        }


        public async Task<IActionResult> UpadtaMemberProfile(decimal? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpadtaMemberProfile(decimal id, [Bind("MemberId,FullName,Email,Password,PhoneNumber,Address,CreatedAt,UpdatedAt,Imageprofileurl,Gender,ImageFile")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (member.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" + member.ImageFile.FileName;
                        string newImagePath = Path.Combine(wwwRootPath, "Images", fileName);

                        using (var fileStream = new FileStream(newImagePath, FileMode.Create))
                        {
                            await member.ImageFile.CopyToAsync(fileStream);
                        }

                        if (!string.IsNullOrEmpty(member.Imageprofileurl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, "Images", member.Imageprofileurl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        member.Imageprofileurl = fileName;
                    }

                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Members");
            }

            return View(member);
        }

        private decimal? GetLoggedInMemberId()
        {
            var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");
            return string.IsNullOrEmpty(memberIdString) ? null : decimal.Parse(memberIdString);
        }

        private bool MemberExists(decimal id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}
