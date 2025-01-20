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
    public class ContactusController : Controller
    {
        private readonly ModelContext _context;

        public ContactusController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult ContactUsForm()
        {
            var contactModel = new Contactu();  // Create an empty instance of the model
            return View(contactModel);  // Pass the model to the view
        }

        public async Task<IActionResult> Index()
        {
            return _context.Contactus != null ?
                        View(await _context.Contactus.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Contactus'  is null.");
        }

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Contactus == null)
            {
                return NotFound();
            }

            var contactu = await _context.Contactus
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contactu == null)
            {
                return NotFound();
            }

            return View(contactu);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,FullName,Email,PhoneNumber,Message,CreatedAt")] Contactu contactu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactu);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your message has been successfully sent!";

                return View();
            }

            return View(contactu);
        }

        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Contactus == null)
            {
                return NotFound();
            }

            var contactu = await _context.Contactus.FindAsync(id);
            if (contactu == null)
            {
                return NotFound();
            }
            return View(contactu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("ContactId,FullName,Email,PhoneNumber,Message,CreatedAt")] Contactu contactu)
        {
            if (id != contactu.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactuExists(contactu.ContactId))
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
            return View(contactu);
        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Contactus == null)
            {
                return NotFound();
            }

            var contactu = await _context.Contactus
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contactu == null)
            {
                return NotFound();
            }

            return View(contactu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Contactus == null)
            {
                return Problem("Entity set 'ModelContext.Contactus'  is null.");
            }
            var contactu = await _context.Contactus.FindAsync(id);
            if (contactu != null)
            {
                _context.Contactus.Remove(contactu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactuExists(decimal id)
        {
            return (_context.Contactus?.Any(e => e.ContactId == id)).GetValueOrDefault();
        }
    }
}
