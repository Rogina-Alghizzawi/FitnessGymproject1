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
            // Retrieve the logged-in member's ID from the session
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

            // Check if the member is logged in
            if (loggedInMemberId == null)
            {
                // If no member is logged in, redirect to a login page or return an appropriate result
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Convert the logged-in member's ID to an integer (or the appropriate type in your case)
            int memberId = int.Parse(loggedInMemberId);

            // Retrieve the payments for the logged-in member
            var modelContext = _context.Payments
                .Include(p => p.Member)
                .Include(p => p.Subscription)
                .Where(p => p.MemberId == memberId);  // Filter payments by logged-in member

            // Return the filtered payments list
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

            // Initialize the model if necessary, especially for new fields
            var payment = new Payment();

            ViewData["MemberId"] = memberId;
            ViewData["SubscriptionId"] = _context.Subscriptions.FirstOrDefault()?.SubscriptionId; // Or any logic for default SubscriptionId

            return View(payment); // Pass an initialized Payment object
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
            ViewData["SubscriptionId"] = _context.Subscriptions.FirstOrDefault()?.SubscriptionId; // Or any logic for default SubscriptionId

            return View(payment);
        }

        // GET: Payments/Delete/5
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

            // Check if the logged-in member is the one associated with the payment
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
            if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
            {
                return Unauthorized(); // Prevent deleting payments of other members
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Payments == null)
            {
                return Problem("Entity set 'ModelContext.Payments'  is null.");
            }

            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            // Ensure that the logged-in member is the one deleting the payment
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
            if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
            {
                return Unauthorized(); // Prevent deleting payments of other members
            }

            // Remove the payment from the database
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            // Retrieve the payment for the logged-in member
            var payment = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.Subscription)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            // Check if the logged-in member is the one associated with the payment
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
            if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
            {
                return Unauthorized(); // Prevent editing payments of other members
            }

            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", payment.SubscriptionId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("PaymentId,MemberId,SubscriptionId,PaymentDate,PaymentStatus,Amount,PaymentMethod,CreatedAt")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure that the logged-in member is the one editing their payment
                    var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
                    if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
                    {
                        return Unauthorized(); // Prevent editing payments of other members
                    }

                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", payment.SubscriptionId);
            return View(payment);
        }

        // Existing methods...

     
        private bool PaymentExists(decimal id)
        {
            return (_context.Payments?.Any(e => e.PaymentId == id)).GetValueOrDefault();
        }
    }
}
