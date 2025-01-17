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
    public class AboutusController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AboutusController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Aboutus
        public async Task<IActionResult> Index()
        {
              return _context.Aboutus != null ? 
                          View(await _context.Aboutus.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Aboutus'  is null.");
        }

        // GET: Aboutus/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Aboutus == null)
            {
                return NotFound();
            }

            var aboutu = await _context.Aboutus
                .FirstOrDefaultAsync(m => m.AboutUsId == id);
            if (aboutu == null)
            {
                return NotFound();
            }

            return View(aboutu);
        }

        // GET: Aboutus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aboutus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AboutUsId,Title,Content,ImageUrl,VideoUrl,ImageFile,CreatedAt,UpdatedAt")] Aboutu aboutu)
        {
            if (!ModelState.IsValid)
            {
                return View(aboutu); // Return the view with validation errors
            }

            try
            {
                if (aboutu.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(aboutu.ImageFile.FileName)}";
                    string imagePath = Path.Combine(wwwRootPath, "images", fileName);

                    // Ensure the images directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                    // Save the uploaded image to the specified path
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await aboutu.ImageFile.CopyToAsync(fileStream);
                    }

                    // Save the relative path to the ImageUrl property
                    aboutu.ImageUrl = $"/images/{fileName}";
                }

                // Set creation and update timestamps
                aboutu.CreatedAt = DateTime.Now;
                aboutu.UpdatedAt = DateTime.Now;

                // Add the record to the database
                _context.Add(aboutu);
                await _context.SaveChangesAsync();

                // Redirect to the index action upon successful creation
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
            }

            return View(aboutu); // Return the view if any errors occur
        }


        // GET: Aboutus/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Aboutus == null)
            {
                return NotFound();
            }

            var aboutu = await _context.Aboutus.FindAsync(id);
            if (aboutu == null)
            {
                return NotFound();
            }
            return View(aboutu);
        }

        // POST: Aboutus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("AboutUsId,Title,Content,ImageUrl,VideoUrl,ImageFile,CreatedAt,UpdatedAt")] Aboutu aboutu)
        {
            if (id != aboutu.AboutUsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handling the Image File upload for Edit
                    if (aboutu.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(aboutu.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await aboutu.ImageFile.CopyToAsync(fileStream);
                        }

                        aboutu.ImageUrl = "/images/" + fileName;  // Update the image URL
                    }

                    aboutu.UpdatedAt = DateTime.Now;  // Set the updated timestamp

                    _context.Update(aboutu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutuExists(aboutu.AboutUsId))
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
                return View(aboutu);
        }

        // GET: Aboutus/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Aboutus == null)
            {
                return NotFound();
            }

            var aboutu = await _context.Aboutus
                .FirstOrDefaultAsync(m => m.AboutUsId == id);
            if (aboutu == null)
            {
                return NotFound();
            }

            return View(aboutu);
        }

        // POST: Aboutus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Aboutus == null)
            {
                return Problem("Entity set 'ModelContext.Aboutus'  is null.");
            }
            var aboutu = await _context.Aboutus.FindAsync(id);
            if (aboutu != null)
            {
                _context.Aboutus.Remove(aboutu);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Aboutuspage()
        {
            // Check if the Aboutus DbSet is not null
            if (_context.Aboutus == null)
            {
                // Return a problem result if the DbSet is null
                return Problem("Entity set 'ModelContext.Aboutus' is null.");
            }

            // Fetch and return the list of "About Us" data asynchronously
            var aboutUsList = await _context.Aboutus.ToListAsync();

            // Pass the fetched list to the view
            return View(aboutUsList);
        }

        private bool AboutuExists(decimal id)
        {
          return (_context.Aboutus?.Any(e => e.AboutUsId == id)).GetValueOrDefault();
        }
    }
}
