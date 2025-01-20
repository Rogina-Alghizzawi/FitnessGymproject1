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
            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");

            if (loggedInMemberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            int memberId = int.Parse(loggedInMemberId);

            var modelContext = _context.Payments
                .Include(p => p.Member)
                .Include(p => p.Subscription)
                .Where(p => p.MemberId == memberId);  

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

            var payment = new Payment();

            ViewData["MemberId"] = memberId;

            return View(payment); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,MemberId,PaymentDate,PaymentStatus,Amount,PaymentMethod,CreatedAt")] Payment payment)
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

            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
            if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
            {
                return Unauthorized(); 
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

            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
            if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
            {
                return Unauthorized(); 
            }

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

            var payment = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.Subscription)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
            if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
            {
                return Unauthorized(); 
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
                    var loggedInMemberId = HttpContext.Session.GetString("LoggedInMemberId");
                    if (loggedInMemberId == null || int.Parse(loggedInMemberId) != payment.MemberId)
                    {
                        return Unauthorized(); 
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



        private bool PaymentExists(decimal id)
        {
            return (_context.Payments?.Any(e => e.PaymentId == id)).GetValueOrDefault();
        }
    }
}