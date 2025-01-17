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
    public class PaymentsController : Controller
    {
        private readonly ModelContext _context;

        public PaymentsController(ModelContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Payments.Include(p => p.Member).Include(p => p.Subscription);
            return View(await modelContext.ToListAsync());
        }

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.Subscription)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        public async Task<IActionResult> Create()
        {
            var loggedInMember = HttpContext.Session.GetString("LoggedInMemberId");

            if (string.IsNullOrEmpty(loggedInMember))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            decimal memberId = Convert.ToDecimal(loggedInMember);

            var member = await _context.Members.FirstOrDefaultAsync(a => a.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            ViewData["MemberId"] = memberId;
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,MemberId,SubscriptionId,PaymentDate,PaymentStatus,Amount,PaymentMethod,CreatedAt")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                var existingPayment = await _context.Payments
                                                    .FirstOrDefaultAsync(p => p.MemberId == payment.MemberId && p.PaymentStatus == "Completed");

                if (existingPayment != null)
                {
                    ModelState.AddModelError(string.Empty, "This member already has a completed payment.");
                    return View(payment);
                }

                _context.Add(payment);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewAvailableMembershipPlans", "MembershipPlans");
            }

            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", payment.MemberId);
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", payment.SubscriptionId);

            return View(payment);
        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.Subscription)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Payments == null)
            {
                return Problem("Entity set 'ModelContext.Payments'  is null.");
            }
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentForWorkout([Bind("PaymentId,MemberId,SubscriptionId,PaymentDate,PaymentStatus,Amount,PaymentMethod,CreatedAt")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                // Ensure the member is not already having a completed payment
                var existingPayment = await _context.Payments
                                                     .FirstOrDefaultAsync(p => p.MemberId == payment.MemberId && p.PaymentStatus == "Completed");

                if (existingPayment != null)
                {
                    ModelState.AddModelError(string.Empty, "This member already has a completed payment.");
                    return View(payment);
                }

                // Ensure the subscription exists and the member is valid
                var subscription = await _context.Subscriptions.FindAsync(payment.SubscriptionId);
                var member = await _context.Members.FindAsync(payment.MemberId);

                if (subscription == null || member == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid subscription or member.");
                    return View(payment);
                }

                // Add the payment
                _context.Add(payment);
                await _context.SaveChangesAsync();

                // Redirect to the appropriate action after successful payment creation
                return RedirectToAction("ViewAvailablePlans", "Members");
            }

            // Re-populate dropdowns in case of validation failure
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", payment.MemberId);
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", payment.SubscriptionId);

            return View(payment);
        }

        private bool PaymentExists(decimal id)
        {
            return (_context.Payments?.Any(e => e.PaymentId == id)).GetValueOrDefault();
        }
    }
}
