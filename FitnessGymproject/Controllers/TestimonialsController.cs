//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using FitnessGymproject.Models;

//namespace FitnessGymproject.Controllers
//{
//    public class TestimonialsController : Controller
//    {
//        private readonly ModelContext _context;

//        public TestimonialsController(ModelContext context)
//        {
//            _context = context;
//        }

//        // GET: Testimonials
//        public async Task<IActionResult> Index()
//        {
//            var modelContext = _context.Testimonials.Include(t => t.Member);
//            return View(await modelContext.ToListAsync());
//        }

//        // GET: Testimonials/Details/5
//        public async Task<IActionResult> Details(decimal? id)
//        {
//            if (id == null || _context.Testimonials == null)
//            {
//                return NotFound();
//            }

//            var testimonial = await _context.Testimonials
//                .Include(t => t.Member)
//                .FirstOrDefaultAsync(m => m.TestimonialId == id);
//            if (testimonial == null)
//            {
//                return NotFound();
//            }

//            return View(testimonial);
//        }

//        // GET: Testimonials/Create
//        public IActionResult Create()
//        {
//            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
//            return View();
//        }

//        // POST: Testimonials/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("TestimonialId,MemberId,Content,Status,CreatedAt")] Testimonial testimonial)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(testimonial);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", testimonial.MemberId);
//            return View(testimonial);
//        }

//        // GET: Testimonials/Edit/5
//        public async Task<IActionResult> Edit(decimal? id)
//        {
//            if (id == null || _context.Testimonials == null)
//            {
//                return NotFound();
//            }

//            var testimonial = await _context.Testimonials.FindAsync(id);
//            if (testimonial == null)
//            {
//                return NotFound();
//            }
//            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", testimonial.MemberId);
//            return View(testimonial);
//        }

//        // POST: Testimonials/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(decimal id, [Bind("TestimonialId,MemberId,Content,Status,CreatedAt")] Testimonial testimonial)
//        {
//            if (id != testimonial.TestimonialId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(testimonial);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!TestimonialExists(testimonial.TestimonialId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", testimonial.MemberId);
//            return View(testimonial);
//        }

//        // GET: Testimonials/Delete/5
//        public async Task<IActionResult> Delete(decimal? id)
//        {
//            if (id == null || _context.Testimonials == null)
//            {
//                return NotFound();
//            }

//            var testimonial = await _context.Testimonials
//                .Include(t => t.Member)
//                .FirstOrDefaultAsync(m => m.TestimonialId == id);
//            if (testimonial == null)
//            {
//                return NotFound();
//            }

//            return View(testimonial);
//        }

//        // POST: Testimonials/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(decimal id)
//        {
//            if (_context.Testimonials == null)
//            {
//                return Problem("Entity set 'ModelContext.Testimonials'  is null.");
//            }
//            var testimonial = await _context.Testimonials.FindAsync(id);
//            if (testimonial != null)
//            {
//                _context.Testimonials.Remove(testimonial);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool TestimonialExists(decimal id)
//        {
//          return (_context.Testimonials?.Any(e => e.TestimonialId == id)).GetValueOrDefault();
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessGymproject.Models;

namespace FitnessGymproject.Controllers
{
    public class TestimonialsController : Controller
    {
        private readonly ModelContext _context;

        public TestimonialsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Testimonials
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Testimonials.Include(t => t.Member);
            return View(await modelContext.ToListAsync());
        }

        // GET: Testimonials/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.Member)
                .FirstOrDefaultAsync(m => m.TestimonialId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }


        public IActionResult Create()
        {
            // Get MemberId from session
            var memberId = HttpContext.Session.GetString("LoggedInMemberId");

            // If MemberId is null, redirect to the login page
            if (string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["MemberId"] = memberId;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("TestimonialId,Content,Status,CreatedAt")] Testimonial testimonial)
        //{
        //    // Get the MemberId from the session
        //    var memberId = HttpContext.Session.GetString("LoggedInMemberId");

        //    // Ensure MemberId is set in the testimonial
        //            if (memberId != null)
        //            {
        //                testimonial.MemberId = int.Parse(memberId);  // Assuming MemberId is an integer
        //    }
        //            else
        //            {
        //                // Redirect to login if MemberId is not in session
        //                return RedirectToAction("Login", "Account");
        //}

        //    if (ModelState.IsValid)
        //    {
        //        testimonial.Status = "Pending";
        //        testimonial.CreatedAt = DateTime.Now;

        //        _context.Add(testimonial);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        // If validation fails, show the form again
        //    return View(testimonial);
        //}


        //GET: Testimonials/Create
        //public IActionResult Create()
        //{
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
        //    return View();
        //}

        // POST: Testimonials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestimonialId,MemberId,Content,Status,CreatedAt")] Testimonial testimonial)
        {
            // Get MemberId from session
            var memberId = HttpContext.Session.GetString("LoggedInMemberId");

            if (memberId != null)
            {
                testimonial.MemberId = int.Parse(memberId);  // Set MemberId from session
            }
            else
            {
                // Redirect to login if MemberId is not in session
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Optionally, load the Member from the database based on MemberId and set the Member navigation property
            var member = await _context.Members.FindAsync(testimonial.MemberId);
            if (member == null)
            {
                // Handle case where Member is not found (optional)
                ModelState.AddModelError("MemberId", "Member not found.");
                return View(testimonial);
            }

            // Set the navigation property manually
            testimonial.Member = member;

            // Set Status and CreatedAt before saving to DB
            testimonial.Status = "Pending";
            testimonial.CreatedAt = DateTime.Now;

            // Check if ModelState is valid before saving the testimonial
            if (ModelState.IsValid)
            {
                // Add and save the Testimonial
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return the form with validation errors
            ViewData["MemberId"] = testimonial.MemberId;
            return View(testimonial); // Will show validation error messages in the view
        }


        // POST: Testimonials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("TestimonialId,MemberId,Content,Status,CreatedAt")] Testimonial testimonial)
        {
            if (id != testimonial.TestimonialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.TestimonialId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", testimonial.MemberId);
            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.Member)
                .FirstOrDefaultAsync(m => m.TestimonialId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Testimonials == null)
            {
                return Problem("Entity set 'ModelContext.Testimonials'  is null.");
            }
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.Status = "Approved";
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: Testimonials/Reject/5
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.Status = "Rejected";
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(decimal id)
        {
            return (_context.Testimonials?.Any(e => e.TestimonialId == id)).GetValueOrDefault();
        }
    }
}

