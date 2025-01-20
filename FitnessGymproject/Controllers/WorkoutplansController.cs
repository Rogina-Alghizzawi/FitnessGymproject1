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
    public class WorkoutplansController : Controller
    {
        private readonly ModelContext _context;

        public WorkoutplansController(ModelContext context)
        {
            _context = context;
        }

        // GET: Workoutplans
       public IActionResult Index()
{
    // Retrieve the list of workout plans
    var workoutPlans = _context.Workoutplans.ToList();

    // If there are no workout plans, log a more descriptive message.
    if (workoutPlans == null || workoutPlans.Count == 0)
    {
        Console.WriteLine("No workout plans found!");
        // Optionally, you can display a message on the view to inform the user.
        TempData["Message"] = "No workout plans available.";
    }

    // Pass the workout plans to the view
    return View(workoutPlans);
}

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Workoutplans == null)
            {
                return NotFound();
            }

            var workoutplan = await _context.Workoutplans
                .Include(w => w.Member)
                .Include(w => w.Trainer)
                .FirstOrDefaultAsync(m => m.WorkoutPlanId == id);
            if (workoutplan == null)
            {
                return NotFound();
            }

            return View(workoutplan);
        }


        // GET: Workoutplans/Create
        public IActionResult Create()
        {
            // Get the logged-in trainer's ID from the session
            decimal TrainerId = Convert.ToDecimal(HttpContext.Session.GetString("TrainerId"));

            // Pass the trainer's ID to the view data to prevent them from choosing the trainer
            ViewData["TrainerId"] = new SelectList(new List<decimal> { TrainerId }, "TrainerId", "TrainerId");

            // Set member dropdown
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            return View();
        }

        // POST: Workoutplans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkoutPlanId,TrainerId,MemberId,PlanName,Exercises,Schedule,Goals,CreatedAt,UpdatedAt")] Workoutplan workoutplan)
        {
            // Ensure that the trainer's ID matches the logged-in trainer's ID
            decimal TrainerId = Convert.ToDecimal(HttpContext.Session.GetString("TrainerId"));
            workoutplan.TrainerId = TrainerId;

            if (ModelState.IsValid)
            {
                _context.Add(workoutplan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workoutplan.MemberId);
            ViewData["TrainerId"] = new SelectList(new List<decimal> { TrainerId }, "TrainerId", "TrainerId", workoutplan.TrainerId);
            return View(workoutplan);
        }
        // GET: Workoutplans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Workoutplans == null)
            {
                return NotFound();
            }

            // Ensure the trainer ID is available in the session
            decimal TrainerId;
            if (!decimal.TryParse(HttpContext.Session.GetString("TrainerId"), out TrainerId))
            {
                return RedirectToAction("Login"); // or handle the missing TrainerId more appropriately
            }

            var workoutplan = await _context.Workoutplans.FindAsync(id);
            if (workoutplan == null)
            {
                return NotFound();
            }

            // Pass the trainer's ID directly to the view data (without a SelectList)
            ViewData["TrainerId"] = TrainerId;

            // Pass the MemberId to allow selection in the dropdown
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workoutplan.MemberId);

            return View(workoutplan);
        }

        // POST: Workoutplans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("WorkoutPlanId,TrainerId,MemberId,PlanName,Exercises,Schedule,Goals,CreatedAt,UpdatedAt")] Workoutplan workoutplan)
        {
            if (id != workoutplan.WorkoutPlanId)
            {
                return NotFound();
            }

            // Get the logged-in trainer's ID from the session
            decimal TrainerId = Convert.ToDecimal(HttpContext.Session.GetString("TrainerId"));

            // Ensure that the workout plan is for the logged-in trainer
            if (workoutplan.TrainerId != TrainerId)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workoutplan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutplanExists(workoutplan.WorkoutPlanId))
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workoutplan.MemberId);
            ViewData["TrainerId"] = new SelectList(new List<decimal> { TrainerId }, "TrainerId", "TrainerId", workoutplan.TrainerId);
            return View(workoutplan);
        }



        //// GET: Workoutplans/Create
        //public IActionResult Create()
        //{
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
        //    ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId");
        //    return View();
        //}

        //// POST: Workoutplans/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("WorkoutPlanId,TrainerId,MemberId,PlanName,Exercises,Schedule,Goals,CreatedAt,UpdatedAt")] Workoutplan workoutplan)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(workoutplan);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workoutplan.MemberId);
        //    ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workoutplan.TrainerId);
        //    return View(workoutplan);
        //}

        //// GET: Workoutplans/Edit/5
        //public async Task<IActionResult> Edit(decimal? id)
        //{
        //    if (id == null || _context.Workoutplans == null)
        //    {
        //        return NotFound();
        //    }

        //    var workoutplan = await _context.Workoutplans.FindAsync(id);
        //    if (workoutplan == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workoutplan.MemberId);
        //    ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workoutplan.TrainerId);
        //    return View(workoutplan);
        //}

        //// POST: Workoutplans/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(decimal id, [Bind("WorkoutPlanId,TrainerId,MemberId,PlanName,Exercises,Schedule,Goals,CreatedAt,UpdatedAt")] Workoutplan workoutplan)
        //{
        //    if (id != workoutplan.WorkoutPlanId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(workoutplan);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!WorkoutplanExists(workoutplan.WorkoutPlanId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workoutplan.MemberId);
        //    ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workoutplan.TrainerId);
        //    return View(workoutplan);
        //}

        // GET: Workoutplans/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Workoutplans == null)
            {
                return NotFound();
            }

            var workoutplan = await _context.Workoutplans
                .Include(w => w.Member)
                .Include(w => w.Trainer)
                .FirstOrDefaultAsync(m => m.WorkoutPlanId == id);
            if (workoutplan == null)
            {
                return NotFound();
            }

            return View(workoutplan);
        }

        // POST: Workoutplans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Workoutplans == null)
            {
                return Problem("Entity set 'ModelContext.Workoutplans'  is null.");
            }
            var workoutplan = await _context.Workoutplans.FindAsync(id);
            if (workoutplan != null)
            {
                _context.Workoutplans.Remove(workoutplan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
 
        private bool WorkoutplanExists(decimal id)
        {
            return (_context.Workoutplans?.Any(e => e.WorkoutPlanId == id)).GetValueOrDefault();
        }
    }
}