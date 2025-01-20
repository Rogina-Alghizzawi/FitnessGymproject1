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
    var workoutPlans = _context.Workoutplans.ToList();

    if (workoutPlans == null || workoutPlans.Count == 0)
    {
        Console.WriteLine("No workout plans found!");
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
            decimal TrainerId = Convert.ToDecimal(HttpContext.Session.GetString("TrainerId"));

            ViewData["TrainerId"] = new SelectList(new List<decimal> { TrainerId }, "TrainerId", "TrainerId");

            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            return View();
        }

        // POST: Workoutplans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkoutPlanId,TrainerId,MemberId,PlanName,Exercises,Schedule,Goals,CreatedAt,UpdatedAt")] Workoutplan workoutplan)
        {
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

            decimal TrainerId;
            if (!decimal.TryParse(HttpContext.Session.GetString("TrainerId"), out TrainerId))
            {
                return RedirectToAction("Login"); 
            }

            var workoutplan = await _context.Workoutplans.FindAsync(id);
            if (workoutplan == null)
            {
                return NotFound();
            }

            ViewData["TrainerId"] = TrainerId;

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

            decimal TrainerId = Convert.ToDecimal(HttpContext.Session.GetString("TrainerId"));

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