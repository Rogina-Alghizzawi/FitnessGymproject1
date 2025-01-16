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

        //public async Task<IActionResult> ViewAvailableMembershipPlans()
        //{
        //    var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

        //    if (loggedInMemberId == null)
        //    {
        //        return RedirectToAction("Login","LoginAndRegister"); // Redirect to login page if not logged in
        //    }

        //    decimal memberId = Convert.ToDecimal(loggedInMemberId);

        //    // Fetch available membership plans (where MemberId is null in the Subscription table)
        //    var availablePlans = await _context.MembershipPlans
        //                                       .Where(mp => mp.Subscriptions.All(s => s.MemberId == null)) // No member subscribed
        //                                       .ToListAsync();  // Ensure you're awaiting the async operation

        //    // Pass the memberId to the view through ViewData
        //    ViewData["MemberId"] = memberId;


        //    return View();  
        //}


        //[HttpPost]
        //public async Task<IActionResult> SubscribeToMembershipPlan(decimal membershipPlanId)
        //{
        //    var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

        //    if (string.IsNullOrEmpty(memberIdString))
        //    {
        //        return RedirectToAction("Login", "LoginAndRegister");
        //    }

        //    var memberId = decimal.Parse(memberIdString);

        //    var membershipPlan = await _context.MembershipPlans.FindAsync(membershipPlanId);
        //    if (membershipPlan != null)
        //    {
        //        var subscription = new Subscription
        //        {
        //            MemberId = memberId,
        //            MembershipPlanId = membershipPlanId,
        //            CreatedAt = DateTime.Now
        //        };

        //        // Add the subscription and save changes
        //        _context.Subscriptions.Add(subscription);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("ViewSubscribedMembershipPlans");
        //}



        public async Task<IActionResult> ViewAvailableMembershipPlans()
        {
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

            if (loggedInMemberId == null)
            {
                return RedirectToAction("Login");
            }

            decimal memberId = Convert.ToDecimal(loggedInMemberId);

            // Fetch available membership plans
            var availablePlans = await _context.MembershipPlans
                                               .Where(mp => mp.Subscriptions.All(s => s.MemberId == null))
                                               .ToListAsync();

            ViewData["MemberId"] = memberId;

            return View(availablePlans);
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeToMembershipPlan(decimal membershipPlanId, string paymentMethod)
        {
            var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal memberId = decimal.Parse(memberIdString);

            var membershipPlan = await _context.MembershipPlans.FindAsync(membershipPlanId);

            if (membershipPlan != null)
            {
                // Check if the member already has an active subscription
                var existingSubscription = _context.Subscriptions
                                                   .FirstOrDefault(s => s.MemberId == memberId && s.MembershipPlanId == membershipPlanId);

                if (existingSubscription != null)
                {
                    ViewBag.Message = "You are already subscribed to this plan.";
                    return RedirectToAction("ViewSubscribedMembershipPlans");
                }

                // Create a new subscription
                var subscription = new Subscription
                {
                    MemberId = memberId,
                    MembershipPlanId = membershipPlanId,
                    PlanName = membershipPlan.PlanName,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(membershipPlan.DurationDays),
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    PaymentStatus = "Completed",
                    TotalPayment = membershipPlan.Price
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                // Create a payment record (PaymentId is needed in the invoice)
                var payment = new Payment
                {
                    MemberId = memberId,
                    SubscriptionId = subscription.SubscriptionId,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Completed",
                    Amount = membershipPlan.Price,
                    PaymentMethod = paymentMethod,
                    CreatedAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();  // Save to get PaymentId

                // Create an invoice record (ensure PaymentId and SubscriptionId are set)
                var invoice = new Invoice
                {
                    SubscriptionId = subscription.SubscriptionId,
                    InvoiceDate = DateTime.Now,
                    TotalAmount = membershipPlan.Price,
                    PaymentStatus = "Paid",
                    PaymentId = payment.PaymentId  // Link the payment
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewSubscribedMembershipPlans");
            }

            return RedirectToAction("ViewAvailableMembershipPlans");
        }

        public async Task<IActionResult> ViewSubscribedMembershipPlans()
        {
            var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal memberId = decimal.Parse(memberIdString);

            var result = from subscription in _context.Subscriptions
                         join invoice in _context.Invoices
                         on subscription.SubscriptionId equals invoice.SubscriptionId
                         where subscription.MemberId == memberId
                         select new
                         {
                             subscription.PlanName,
                             subscription.StartDate,
                             subscription.EndDate,
                             invoice.InvoiceId,
                             invoice.TotalAmount,
                             invoice.PaymentStatus
                         };

            return View(await result.ToListAsync());
        }


        private bool MembershipPlanExists(decimal id)
        {
          return (_context.MembershipPlans?.Any(e => e.MembershipPlanId == id)).GetValueOrDefault();
        }
    }
}
