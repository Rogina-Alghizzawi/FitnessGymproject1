using Microsoft.AspNetCore.Mvc;
using FitnessGymproject.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FitnessGymproject.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;

        public LoginAndRegisterController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(Member member)
        {
            if (ModelState.IsValid)
            {
                if (member.ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, member.ImageFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await member.ImageFile.CopyToAsync(stream);
                    }

                    member.Imageprofileurl = "/images/" + member.ImageFile.FileName;
                }

                member.CreatedAt = DateTime.Now;
                member.UpdatedAt = DateTime.Now;

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(member);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email && a.Password == password);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == email && t.Password == password);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email && m.Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("LoggedInAdminId", admin.AdminId.ToString());
                HttpContext.Session.SetString("LoggedInAdminName", admin.FullName);
                HttpContext.Session.SetString("LoggedInAdminEmail", admin.Email);
                HttpContext.Session.SetString("LoggedInAdminGender", admin.Gender);

                return RedirectToAction("Index", "Adminpage");
            }
            else if (trainer != null)
            {
                HttpContext.Session.SetString("TrainerId", trainer.TrainerId.ToString());
                HttpContext.Session.SetString("TrainerName", trainer.FullName);
                HttpContext.Session.SetString("TrainerEmail", trainer.Email);
                HttpContext.Session.SetString("TrainerGender", trainer.Gender);
                HttpContext.Session.SetString("TrainerPassword", trainer.Password);
                HttpContext.Session.SetString("TrainerPhoneNumber", trainer.PhoneNumber);
                HttpContext.Session.SetString("TrainerSpecialization", trainer.Specialization);
                HttpContext.Session.SetString("TrainerBio", trainer.Bio);
                HttpContext.Session.SetString("TrainerImage", trainer.Imageprofileurl);

                return RedirectToAction("Index", "TrainerPage");
            }
            else if (member != null)
            {
                HttpContext.Session.SetString("LoggedInMemberId", member.MemberId.ToString());
                HttpContext.Session.SetString("LoggedInMemberName", member.FullName);
                HttpContext.Session.SetString("LoggedInMemberEmail", member.Email);
                HttpContext.Session.SetString("LoggedInMemberGender", member.Gender);

                if (!string.IsNullOrEmpty(member.PhoneNumber))
                {
                    HttpContext.Session.SetString("LoggedInMemberPhone", member.PhoneNumber);
                }

                if (!string.IsNullOrEmpty(member.Address))
                {
                    HttpContext.Session.SetString("LoggedInMemberAddress", member.Address);
                }

                if (member.CreatedAt.HasValue)
                {
                    HttpContext.Session.SetString("LoggedInMemberCreatedAt", member.CreatedAt.Value.ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(member.Imageprofileurl))
                {
                    HttpContext.Session.SetString("LoggedInMemberProfileImage", member.Imageprofileurl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid login credentials.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
