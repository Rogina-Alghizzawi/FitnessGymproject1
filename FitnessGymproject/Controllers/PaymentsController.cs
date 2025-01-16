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

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Payments.Include(p => p.Member).Include(p => p.Subscription);
            return View(await modelContext.ToListAsync());
        }

        // GET: Payments/Details/5
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

        // GET: Payments/Create
        public async Task<IActionResult> Create()
        {
            // Check if the member is logged in by retrieving the session value
            var loggedInMember = HttpContext.Session.GetString("LoggedInMemberId");

            // If the member ID is not present, redirect to the login page
            if (string.IsNullOrEmpty(loggedInMember))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Convert the member ID from the session to decimal
            decimal memberId = Convert.ToDecimal(loggedInMember);

            // Retrieve the member from the database
            var member = await _context.Members.FirstOrDefaultAsync(a => a.MemberId == memberId);

            // If the member doesn't exist, return a NotFound result
            if (member == null)
            {
                return NotFound();
            }

            // Set the MemberId as a hidden value to be passed with the form submission
            ViewData["MemberId"] = memberId;  // Pass the logged-in member's ID to the view
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId");

            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,MemberId,SubscriptionId,PaymentDate,PaymentStatus,Amount,PaymentMethod,CreatedAt")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                // Check if the member already has an existing payment
                var existingPayment = await _context.Payments
                                                    .FirstOrDefaultAsync(p => p.MemberId == payment.MemberId && p.PaymentStatus == "Completed");

                if (existingPayment != null)
                {
                    // If a completed payment exists for the member, display a message and prevent the creation of a new payment
                    ModelState.AddModelError(string.Empty, "This member already has a completed payment.");
                    return View(payment);  // Return to the view with the error message
                }

                // No existing payment, so proceed with the creation of the new payment
                _context.Add(payment);
                await _context.SaveChangesAsync();

                // Redirect to ViewAvailableMembershipPlans action in MembershipPlansController
                return RedirectToAction("ViewAvailableMembershipPlans", "MembershipPlans");
            }

            // If the model state is invalid, return to the same view
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", payment.MemberId);
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", payment.SubscriptionId);

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
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(decimal id)
        {
          return (_context.Payments?.Any(e => e.PaymentId == id)).GetValueOrDefault();
        }
    }
}
