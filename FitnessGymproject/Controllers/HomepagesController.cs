using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessGymproject.Models;
using Microsoft.AspNetCore.Hosting;

namespace FitnessGymproject.Controllers
{
    public class HomepagesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomepagesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Homepages != null ?
                        View(await _context.Homepages.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Homepages' is null.");
        }

        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Homepages == null)
            {
                return NotFound();
            }

            var homepage = await _context.Homepages
                .FirstOrDefaultAsync(m => m.HomepageId == id);
            if (homepage == null)
            {
                return NotFound();
            }

            return View(homepage);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HomepageId,Title,Description,LogoUrl,VideoUrl,CreatedAt,UpdatedAt,ImageFile")] Homepage homepage)
        {
            if (ModelState.IsValid)
            {
                if (homepage.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(homepage.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homepage.ImageFile.CopyToAsync(fileStream);
                    }

                    homepage.LogoUrl = "/images/" + fileName;
                }

                homepage.CreatedAt = DateTime.Now;
                homepage.UpdatedAt = DateTime.Now;

                _context.Add(homepage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(homepage);
        }

        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Homepages == null)
            {
                return NotFound();
            }

            var homepage = await _context.Homepages.FindAsync(id);
            if (homepage == null)
            {
                return NotFound();
            }
            return View(homepage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("HomepageId,Title,Description,LogoUrl,VideoUrl,CreatedAt,UpdatedAt,ImageFile")] Homepage homepage)
        {
            if (id != homepage.HomepageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (homepage.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(homepage.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await homepage.ImageFile.CopyToAsync(fileStream);
                        }

                        homepage.LogoUrl = "/images/" + fileName;
                    }

                    homepage.UpdatedAt = DateTime.Now;

                    _context.Update(homepage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomepageExists(homepage.HomepageId))
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
            return View(homepage);
        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Homepages == null)
            {
                return NotFound();
            }

            var homepage = await _context.Homepages
                .FirstOrDefaultAsync(m => m.HomepageId == id);
            if (homepage == null)
            {
                return NotFound();
            }

            return View(homepage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Homepages == null)
            {
                return Problem("Entity set 'ModelContext.Homepages' is null.");
            }
            var homepage = await _context.Homepages.FindAsync(id);
            if (homepage != null)
            {
                _context.Homepages.Remove(homepage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredContent(string title)
        {
            var homePageContent = await _context.Homepages
                                                .Where(h => h.Title == title)
                                                .ToListAsync();
            return View(homePageContent);
        }

        private bool HomepageExists(decimal id)
        {
            return (_context.Homepages?.Any(e => e.HomepageId == id)).GetValueOrDefault();
        }
    }
}
