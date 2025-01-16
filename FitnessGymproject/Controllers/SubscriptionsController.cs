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

        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "FullName");
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans, "MembershipPlanId", "PlanName");
            ViewData["WorkoutPlanId"] = new SelectList(_context.Workoutplans, "WorkoutPlanId", "PlanName");
            return View();
        }

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

        public IActionResult Subscribe(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipPlan = _context.MembershipPlans.FirstOrDefault(m => m.MembershipPlanId == id);
            if (membershipPlan == null)
            {
                return NotFound();
            }

            var userIdString = HttpContext.Session.GetString("LoggedInMemberId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            decimal userId;
            if (!decimal.TryParse(userIdString, out userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var existingSubscription = _context.Subscriptions.FirstOrDefault(s => s.MemberId == userId && s.MembershipPlanId == membershipPlan.MembershipPlanId && s.EndDate > DateTime.Now);
            if (existingSubscription != null)
            {
                TempData["Message"] = "You already have an active subscription for this plan!";
                return RedirectToAction("Index", "MembershipPlans");
            }

            var subscription = new Subscription
            {
                MembershipPlanId = membershipPlan.MembershipPlanId,
                MemberId = userId,
                CreatedAt = DateTime.Now,
                EndDate = DateTime.Now.AddDays(membershipPlan.DurationDays)
            };

            _context.Add(subscription);
            _context.SaveChanges();

            TempData["Message"] = "Subscription successful!";
            return RedirectToAction("Index", "Subscriptions");
        }

        private bool SubscriptionExists(decimal id)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }
    }
}
