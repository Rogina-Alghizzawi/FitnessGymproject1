using FitnessGymproject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessGymproject.Controllers
{

    public class TrainerPageController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainerPageController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            var loggedInTrainerId = HttpContext.Session.GetString("TrainerId");

            if (string.IsNullOrEmpty(loggedInTrainerId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal TrainerId = Convert.ToDecimal(loggedInTrainerId);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == TrainerId);

            if (trainer == null)
            {
                return NotFound();
            }

            ViewData["TrainerName"] = trainer.FullName;
            ViewData["TrainerId"] = trainer.TrainerId;

            return View();
        }

        public async Task<IActionResult> TrainerProfile()
        {
            var loggedInTrainerId = HttpContext.Session.GetString("TrainerId");

            if (string.IsNullOrEmpty(loggedInTrainerId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal TrainerId = Convert.ToDecimal(loggedInTrainerId);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == TrainerId);

            if (trainer == null)
            {
                return NotFound();
            }


            var trainerId = Convert.ToDecimal(loggedInTrainerId);

           

            ViewData["TrainerImage"] = trainer.Imageprofileurl;
            ViewData["TrainerId"] = trainer.TrainerId;
            ViewData["TrainerEmail"] = trainer.Email;
            ViewData["TrainerName"] = trainer.FullName;
            ViewData["TrainerGender"] = trainer.Gender;
            ViewData["TrainerPassword"] = trainer.Password;
            ViewData["TrainerSpecialization"] = trainer.Specialization;
            ViewData["TrainerPhoneNumber"] = trainer.PhoneNumber;
            ViewData["TrainerBio"] = trainer.Bio;






            return View(trainer);
        }


        // GET: Trainers/Edit
       
        public async Task<IActionResult> Edit()
        {

    



            decimal TrainerId = Convert.ToDecimal(HttpContext.Session.GetString("TrainerId"));
            var trainer = await _context.Trainers.FindAsync(TrainerId);



            if (trainer == null)
            {
                return NotFound();
            }
       
         

            ViewData["TrainerName"] = trainer.FullName;
            ViewData["TrainerEmail"] = trainer.Email;
            ViewData["TrainerGender"] = trainer.Gender;
            ViewData["TrainerId"] = trainer.TrainerId;

            return View(trainer);
        }


        // POST: Trainers/Edit
        // POST: Trainers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("TrainerId,FullName,Email,Password,PhoneNumber,Specialization,Bio,CreatedAt,UpdatedAt,Imageprofileurl,Gender,ImageFile")] Trainer trainer)
        {
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var loggedInTrainerId = HttpContext.Session.GetString("TrainerId");
                    if (loggedInTrainerId != trainer.TrainerId.ToString())
                    {
                        return Unauthorized();
                    }

                    if (trainer.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;

                        if (!string.IsNullOrEmpty(trainer.Imageprofileurl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, trainer.Imageprofileurl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                    
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(trainer.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath, "images", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await trainer.ImageFile.CopyToAsync(fileStream);
                        }

                        trainer.Imageprofileurl = "/images/" + fileName;
                    }

                    _context.Update(trainer);
                    await _context.SaveChangesAsync();

                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("TrainerName", trainer.FullName);
                HttpContext.Session.SetString("TrainerEmail", trainer.Email);
                HttpContext.Session.SetString("TrainerGender", trainer.Gender);

                return RedirectToAction(nameof(TrainerProfile));
            }

            return View(trainer);
        }


        public async Task<IActionResult> ViewMembers()
        {
            var loggedInTrainerId = HttpContext.Session.GetString("TrainerId");

            if (string.IsNullOrEmpty(loggedInTrainerId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal TrainerId = Convert.ToDecimal(loggedInTrainerId);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == TrainerId);

            if (trainer == null)
            {
                return NotFound();
            }

            var members = await _context.Members.ToListAsync();

            return View(members);  
        }


        // GET: Trainers/ViewAll
        public async Task<IActionResult> ViewAllTrainers()
        {
            var trainers = await _context.Trainers.ToListAsync();

            if (trainers == null || !trainers.Any())
            {
                return NotFound("No trainers found.");
            }

            return View(trainers); 
        }

        private bool TrainerExists(decimal id)
        {
            return (_context.Trainers?.Any(e => e.TrainerId == id)).GetValueOrDefault();
        }
    }
}
