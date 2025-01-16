using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessGymproject.Models;
using Microsoft.Extensions.Configuration;

namespace FitnessGymproject.Controllers
{
    public class MembershipPlansController : Controller
    {
        private readonly ModelContext _context;
        private readonly IConfiguration _configuration;

        public MembershipPlansController(ModelContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

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
            var userActiveSubscriptions = await _context.Subscriptions
                                                        .Where(s => s.MemberId == memberId && s.Status == "Active")
                                                        .Select(s => s.MembershipPlanId)
                                                        .ToListAsync();

            var availablePlans = await _context.MembershipPlans
                                               .Where(mp => !_context.Subscriptions
                                                                      .Any(s => s.MembershipPlanId == mp.MembershipPlanId && s.MemberId == memberId && s.Status == "Active")
                                                       && !userActiveSubscriptions.Contains(mp.MembershipPlanId))
                                               .ToListAsync();

            var userPaymentMethods = await _context.Payments
                                                   .Where(p => p.MemberId == memberId && !string.IsNullOrEmpty(p.PaymentMethod))
                                                   .Select(p => p.PaymentMethod)
                                                   .Distinct()
                                                   .ToListAsync();

            var paymentMethodsText = userPaymentMethods.Any() ? string.Join(", ", userPaymentMethods) : "No payment methods available";

            ViewData["MemberId"] = memberId;
            ViewData["UserPaymentMethodsText"] = paymentMethodsText;

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

            // Fetch the membership plan first
            var membershipPlan = await _context.MembershipPlans.FindAsync(membershipPlanId);

            if (membershipPlan == null)
            {
                return RedirectToAction("ViewAvailableMembershipPlans");
            }

            // Fetch the user's most recent payment record
            var currentPayment = await _context.Payments
                                               .Where(p => p.MemberId == memberId && !string.IsNullOrEmpty(p.PaymentMethod))
                                               .OrderByDescending(p => p.PaymentDate)
                                               .FirstOrDefaultAsync();

            if (currentPayment == null)
            {
                if (currentPayment == null)
                {
                    TempData["ErrorMessage"] = "Please add a payment method to proceed.";
                    return RedirectToAction("Create", "Payments"); // Redirect to a proper action
                }

            }

            // Check if the user has enough balance
            if (currentPayment.Amount < membershipPlan.Price)
            {
                // If balance is insufficient, show an error message and stay on the same page
                ViewBag.ErrorMessage = "Your payment balance is insufficient. Please add more funds to proceed.";
                return View("ViewSubscribedMembershipPlans");  
            }

            // Deduct the amount from the user's balance
            currentPayment.Amount -= membershipPlan.Price;
            _context.Payments.Update(currentPayment);
            await _context.SaveChangesAsync();

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

            // Add the payment record
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
            await _context.SaveChangesAsync();

            // Create an invoice record
            var invoice = new Invoice
            {
                SubscriptionId = subscription.SubscriptionId,
                InvoiceDate = DateTime.Now,
                TotalAmount = membershipPlan.Price,
                PaymentStatus = "Paid",
                PaymentId = payment.PaymentId
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewSubscribedMembershipPlans");
        }



        public async Task<IActionResult> ViewSubscribedMembershipPlans()
        {
            var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(memberIdString))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal memberId = decimal.Parse(memberIdString);

            // Query to join Subscription, Invoice, and Payment
            var result = await (from subscription in _context.Subscriptions
                                join invoice in _context.Invoices
                                on subscription.SubscriptionId equals invoice.SubscriptionId
                                join payment in _context.Payments
                                on subscription.SubscriptionId equals payment.SubscriptionId into paymentGroup
                                from payment in paymentGroup.DefaultIfEmpty()
                                where subscription.MemberId == memberId
                                select new
                                {
                                    subscription.PlanName,
                                    subscription.StartDate,
                                    subscription.EndDate,
                                    invoice.InvoiceId,
                                    invoice.TotalAmount,
                                    invoice.PaymentStatus,
                                    CustomPaymentStatus = payment == null ? "No Payment" : (payment.PaymentStatus == "Completed" ? "Paid" : "Pending")
                                }).ToListAsync();

            // Debugging: Check if result contains any data
            if (result.Count == 0)
            {
                ViewBag.AlertMessage = "No subscriptions found for this member.";
            }
            else
            {
                Console.WriteLine($"Found {result.Count} subscriptions for member {memberId}");
            }

            bool hasPendingPayments = result.Any(s => s.CustomPaymentStatus == "No Payment");

            if (hasPendingPayments)
            {
                ViewBag.AlertMessage = "You have subscriptions without payments. Please complete the payment.";
            }

            // Pass result to ViewData or ViewBag
            ViewBag.Subscriptions = result;

            return View();
        }






        private bool MembershipPlanExists(decimal id)
        {
            return (_context.MembershipPlans?.Any(e => e.MembershipPlanId == id)).GetValueOrDefault();
        }
    }
}