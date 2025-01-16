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

        public async Task<IActionResult> Index()
        {
            return _context.MembershipPlans != null ?
                        View(await _context.MembershipPlans.ToListAsync()) :
                        Problem("Entity set 'ModelContext.MembershipPlans'  is null.");
        }

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

        public IActionResult Create()
        {
            return View();
        }

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

            var membershipPlan = await _context.MembershipPlans.FindAsync(membershipPlanId);

            if (membershipPlan == null)
            {
                return RedirectToAction("ViewAvailableMembershipPlans");
            }

            var currentPayment = await _context.Payments
                                               .Where(p => p.MemberId == memberId && !string.IsNullOrEmpty(p.PaymentMethod))
                                               .OrderByDescending(p => p.PaymentDate)
                                               .FirstOrDefaultAsync();

            if (currentPayment == null)
            {
                TempData["ErrorMessage"] = "Please add a payment method to proceed.";
                return RedirectToAction("Create", "Payments");
            }

            if (currentPayment.Amount < membershipPlan.Price)
            {
                ViewBag.ErrorMessage = "Your payment balance is insufficient. Please add more funds to proceed.";
                return View("ViewSubscribedMembershipPlans");
            }

            currentPayment.Amount -= membershipPlan.Price;
            _context.Payments.Update(currentPayment);
            await _context.SaveChangesAsync();

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

            ViewBag.Subscriptions = result;

            return View();
        }

        private bool MembershipPlanExists(decimal id)
        {
            return (_context.MembershipPlans?.Any(e => e.MembershipPlanId == id)).GetValueOrDefault();
        }
    }
}
