using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessGymproject.Models;
using Microsoft.AspNetCore.Hosting;

namespace FitnessGymproject.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            return _context.Admins != null ?
                        View(await _context.Admins.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Admins'  is null.");
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,FullName,Email,Password,CreatedAt,UpdatedAt,Gender,Imageprofileurl,ImageFile")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                if (admin.ImageFile != null)
                {
                    // Save the uploaded image
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(admin.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await admin.ImageFile.CopyToAsync(fileStream);
                    }

                    // Set the Imageprofileurl property
                    admin.Imageprofileurl = "/images/" + fileName;
                }

                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }




        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("AdminId,FullName,Email,Password,CreatedAt,UpdatedAt,Gender,Imageprofileurl,ImageFile")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (admin.ImageFile != null)
                    {
                        // Save the uploaded image
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(admin.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await admin.ImageFile.CopyToAsync(fileStream);
                        }

                        // Set the Imageprofileurl property
                        admin.Imageprofileurl = "/images/" + fileName;
                    }

                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
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
            return View(admin);
        }


        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Admins == null)
            {
                return Problem("Entity set 'ModelContext.Admins'  is null.");
            }
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool AdminExists(decimal id)
        {
            return (_context.Admins?.Any(e => e.AdminId == id)).GetValueOrDefault();
        }
    }
}