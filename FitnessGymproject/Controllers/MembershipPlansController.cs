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
    public class MembershipPlansController : Controller
    {
        private readonly ModelContext _context;

        public MembershipPlansController(ModelContext context)
        {
            _context = context;
        }

        // GET: MembershipPlans
        public async Task<IActionResult> Index()
        {
              return _context.MembershipPlans != null ? 
                          View(await _context.MembershipPlans.ToListAsync()) :
                          Problem("Entity set 'ModelContext.MembershipPlans'  is null.");
        }

        // GET: MembershipPlans/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.MembershipPlans == null)
            {
                return NotFound();
            }

            var membershipPlan = await _context.MembershipPlans
                .FirstOrDefaultAsync(m => m.MembershipPlanId == id);
            if (membershipPlan == null)
            {
                return NotFound();
            }

            return View(membershipPlan);
        }

        // GET: MembershipPlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MembershipPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipPlanId,PlanName,PlanDescription,IncludedServices,Price,DurationDays,CreatedAt,UpdatedAt")] MembershipPlan membershipPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membershipPlan);
        }

        // GET: MembershipPlans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.MembershipPlans == null)
            {
                return NotFound();
            }

            var membershipPlan = await _context.MembershipPlans.FindAsync(id);
            if (membershipPlan == null)
            {
                return NotFound();
            }
            return View(membershipPlan);
        }

        // POST: MembershipPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("MembershipPlanId,PlanName,PlanDescription,IncludedServices,Price,DurationDays,CreatedAt,UpdatedAt")] MembershipPlan membershipPlan)
        {
            if (id != membershipPlan.MembershipPlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipPlanExists(membershipPlan.MembershipPlanId))
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
            return View(membershipPlan);
        }

        // GET: MembershipPlans/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.MembershipPlans == null)
            {
                return NotFound();
            }

            var membershipPlan = await _context.MembershipPlans
                .FirstOrDefaultAsync(m => m.MembershipPlanId == id);
            if (membershipPlan == null)
            {
                return NotFound();
            }

            return View(membershipPlan);
        }

        // POST: MembershipPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.MembershipPlans == null)
            {
                return Problem("Entity set 'ModelContext.MembershipPlans'  is null.");
            }
            var membershipPlan = await _context.MembershipPlans.FindAsync(id);
            if (membershipPlan != null)
            {
                _context.MembershipPlans.Remove(membershipPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipPlanExists(decimal id)
        {
          return (_context.MembershipPlans?.Any(e => e.MembershipPlanId == id)).GetValueOrDefault();
        }
    }
}
