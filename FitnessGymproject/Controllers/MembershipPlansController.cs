using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessGymproject.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.Mail;

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
            // Get all available membership plans that are not already subscribed to by any member
            var availablePlans = await _context.MembershipPlans
                                               .Where(mp => !_context.Subscriptions
                                                                      .Any(s => s.MembershipPlanId == mp.MembershipPlanId && s.Status == "Active"))
                                               .ToListAsync();

            // Get all distinct payment methods from the database (not based on specific user anymore)
            var userPaymentMethods = await _context.Payments
                                                   .Where(p => !string.IsNullOrEmpty(p.PaymentMethod))
                                                   .Select(p => p.PaymentMethod)
                                                   .Distinct()
                                                   .ToListAsync();

            // Join the payment methods into a string for display
            var paymentMethodsText = userPaymentMethods.Any() ? string.Join(", ", userPaymentMethods) : "No payment methods available";

            // Pass the payment methods text to the View
            ViewData["UserPaymentMethodsText"] = paymentMethodsText;

            return View(availablePlans);
        }
        [HttpPost]
public async Task<IActionResult> SubscribeToMembershipPlan(decimal membershipPlanId, string paymentMethod)
{
    QuestPDF.Settings.License = LicenseType.Community;

    var memberIdString = HttpContext.Session.GetString("LoggedInMemberId");

    if (string.IsNullOrEmpty(memberIdString))
    {
        return RedirectToAction("Login", "LoginAndRegister");
    }

    decimal memberId = decimal.Parse(memberIdString);

    // Check if the user already has an active subscription
    var existingSubscription = await _context.Subscriptions
                                             .FirstOrDefaultAsync(s => s.MemberId == memberId && s.Status == "Active");

    if (existingSubscription != null)
    {
        TempData["ErrorMessage"] = $"You already have an active subscription to the {existingSubscription.PlanName} plan.";
        return RedirectToAction("ViewSubscribedMembershipPlans");
    }

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
            var member = _context.Members.SingleOrDefault(m => m.MemberId == memberId);

            _context.Invoices.Add(invoice);
    await _context.SaveChangesAsync();

            if (payment.PaymentStatus == "Completed")
            {
                string content = $"Dear {member.FullName},\n\n" +
$"We are pleased to inform you that your payment for the {subscription.MembershipPlan.PlanName} subscription has been successfully completed.\n\n" +
$"Plan Details:\n" +
$"- Description: {subscription.MembershipPlan.PlanDescription}\n" +
$"- Included Services: {subscription.MembershipPlan.IncludedServices ?? "Not specified"}\n" +
$"- Duration: {subscription.MembershipPlan.DurationDays} days\n" +
$"- Price: ${subscription.MembershipPlan.Price}\n\n" +
"Thank you for choosing Fitness Gym. We are excited to have you with us and look forward to supporting you on your fitness journey!\n\n" +
"Best Regards,\nFitness Team";



                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Content().Column(column =>
                        {
                            column.Item().Text("Payment Confirmation").FontSize(20).Bold().AlignCenter();
                            column.Item().Text(content).FontSize(12);
                        });
                    });
                });

                using (var memoryStream = new MemoryStream())
                {
                    document.GeneratePdf(memoryStream);
                    var pdfBytes = memoryStream.ToArray();

                    var smtpServer = _configuration["SmtpSettings:Host"];
                    var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                    var senderEmail = _configuration["SmtpSettings:Username"];
                    var senderPassword = _configuration["SmtpSettings:Password"];
                    try
                    {
                        using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                        {
                            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                            smtpClient.EnableSsl = true;

                            var mailMessage = new MailMessage
                            {
                                From = new MailAddress(senderEmail),
                                Subject = "Payment Confirmation",
                                Body = $"Dear {member.FullName},\n\nYour payment for {subscription.MembershipPlan.PlanName} has been confirmed.\n\nBest Regards,\n Fitness Team",
                                IsBodyHtml = false
                            };

                            mailMessage.To.Add(member.Email);
                            mailMessage.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "PaymentConfirmation.pdf"));

                            smtpClient.Send(mailMessage);

                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = $"Payment successful, but the email could not be sent. Error: {ex.Message}";
                    }
                    TempData["Message"] = "Payment completed and confirmation email sent.";
                    return File(pdfBytes, "application/pdf", "PaymentConfirmation.pdf");
                }


            }
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
