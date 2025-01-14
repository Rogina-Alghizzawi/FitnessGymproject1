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
    public class SubscriptionsController : Controller
    {
        private readonly ModelContext _context;

        public SubscriptionsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index(string searchString, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<Subscription> subscriptions = _context.Subscriptions
     .Include(s => s.Member)
     .Include(s => s.MembershipPlan)
     .Include(s => s.WorkoutPlan);


            if (!string.IsNullOrEmpty(searchString))
            {
                subscriptions = subscriptions.Where(s => s.Member != null && s.Member.FullName.Contains(searchString));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                subscriptions = subscriptions.Where(s => s.StartDate >= startDate && s.EndDate <= endDate);
            }

            return View(await subscriptions.ToListAsync());
        }

        // GET: Subscriptions/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FullName");
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans, "MembershipPlanId", "PlanName");
            ViewData["WorkoutPlanId"] = new SelectList(_context.Workoutplans, "WorkoutPlanId", "PlanName");
            return View();
        }

        // POST: Subscriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscriptionId,MemberId,MembershipPlanId,WorkoutPlanId,StartDate,EndDate,TotalPayment,Status,PaymentStatus")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FullName", subscription.MemberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans, "MembershipPlanId", "PlanName", subscription.MembershipPlanId);
            ViewData["WorkoutPlanId"] = new SelectList(_context.Workoutplans, "WorkoutPlanId", "PlanName", subscription.WorkoutPlanId);
            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FullName", subscription.MemberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans, "MembershipPlanId", "PlanName", subscription.MembershipPlanId);
            ViewData["WorkoutPlanId"] = new SelectList(_context.Workoutplans, "WorkoutPlanId", "PlanName", subscription.WorkoutPlanId);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("SubscriptionId,MemberId,MembershipPlanId,WorkoutPlanId,StartDate,EndDate,TotalPayment,Status,PaymentStatus")] Subscription subscription)
        {
            if (id != subscription.SubscriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.SubscriptionId))
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FullName", subscription.MemberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans, "MembershipPlanId", "PlanName", subscription.MembershipPlanId);
            ViewData["WorkoutPlanId"] = new SelectList(_context.Workoutplans, "WorkoutPlanId", "PlanName", subscription.WorkoutPlanId);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.MembershipPlan)
                .Include(s => s.WorkoutPlan)
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);

            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(decimal id)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }
    }
}
