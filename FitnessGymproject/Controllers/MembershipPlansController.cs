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

        public async Task<IActionResult> ViewAvailableMembershipPlans()
        {
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

            if (loggedInMemberId == null)
            {
                return RedirectToAction("Login"); // Redirect to login page if not logged in
            }

            decimal memberId = Convert.ToDecimal(loggedInMemberId);

            // Fetch available membership plans (where MemberId is null in the Subscription table)
            var availablePlans = await _context.MembershipPlans
                                               .Where(mp => mp.Subscriptions.All(s => s.MemberId == null)) // No member subscribed
                                               .ToListAsync();  // Ensure you're awaiting the async operation

            // Pass the memberId to the view through ViewData
            ViewData["MemberId"] = memberId;

            // Return the view with available plans
            return View();  // Pass the awaited result
        }


        [HttpPost]
        public async Task<IActionResult> SubscribeToMembershipPlan(decimal membershipPlanId)
        {
            var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var memberId = decimal.Parse(memberIdString);

            var membershipPlan = await _context.MembershipPlans.FindAsync(membershipPlanId);
            if (membershipPlan != null)
            {
                // Assuming you have a Subscription model to handle member subscriptions to plans
                var subscription = new Subscription
                {
                    MemberId = memberId,
                    MembershipPlanId = membershipPlanId,
                    CreatedAt = DateTime.Now
                };

                // Add the subscription and save changes
                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewSubscribedMembershipPlans");
        }

        public IActionResult ViewSubscribedMembershipPlans()
        {
            // Retrieve the member ID from the session
            string memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                // If no member is logged in, handle appropriately (e.g., redirect to login page)
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Convert the string to a decimal (since MemberId is decimal)
            decimal memberId = Convert.ToDecimal(memberIdString);

            // Query the database for all membership plans the member is subscribed to
            var membershipPlans = _context.Subscriptions
                                          .Where(s => s.MemberId == memberId)
                                          .Include(s => s.MembershipPlan)  // Include the MembershipPlan details
                                          .Select(s => s.MembershipPlan)
                                          .ToList();

            if (membershipPlans == null || !membershipPlans.Any())
            {
                // Handle case where no membership plans are subscribed (e.g., show a message)
                ViewBag.Message = "No membership plans subscribed.";
                return View();
            }

            // Return the list of membership plans to the view
            return View(membershipPlans);
        }




        private bool MembershipPlanExists(decimal id)
        {
          return (_context.MembershipPlans?.Any(e => e.MembershipPlanId == id)).GetValueOrDefault();
        }
    }
}
