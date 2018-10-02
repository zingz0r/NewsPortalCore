using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Models.Entiy;
using NewsPortal.ViewModels;

namespace NewsPortal.Controllers
{
    public class PicturesController : Controller
    {
        private readonly NewsPortalContext _context;

        public PicturesController(NewsPortalContext context)
        {
            _context = context;
        }

        // GET: Pictures
        public async Task<IActionResult> Index()
        {
            var newsPortalContext = _context.Picture.Include(p => p.Article);
            return View(await newsPortalContext.ToListAsync());
        }

        // GET: Pictures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picture = await _context.Picture
                .Include(p => p.Article)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (picture == null)
            {
                return NotFound();
            }

            return View(picture);
        }

        // GET: Pictures/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.Article, "Id", "Summary");
            return View();
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PictureUploadViewModel pictureUploadViewModel)
        {
            if (pictureUploadViewModel.SmallImageData == null)
            {
                ModelState.AddModelError("SmallImageData", "Please select an image!");
            }
            if (pictureUploadViewModel.LargeImageData == null)
            {
                ModelState.AddModelError("LargeImageData", "Please select an image!");
            }

            if (ModelState.IsValid)
            {

                Picture picture = new Picture()
                {
                    ArticleId = pictureUploadViewModel.ArticleId,
                };

                // Large Image
                using (var memoryStream = new MemoryStream())
                {
                    await pictureUploadViewModel.LargeImageData.CopyToAsync(memoryStream);
                    picture.LargeImageData = memoryStream.ToArray();
                }

                // Small image
                using (var memoryStream = new MemoryStream())
                {
                    await pictureUploadViewModel.SmallImageData.CopyToAsync(memoryStream);
                    picture.SmallImageData = memoryStream.ToArray();
                }

                _context.Add(picture);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, "Id", "Summary");

            return View();
        }

        // GET: Pictures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picture = await _context.Picture.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, "Id", "Summary", picture.ArticleId);
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LargeImageData,SmallImageData,ArticleId")] Picture picture)
        {
            if (id != picture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(picture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PictureExists(picture.Id))
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
            ViewData["ArticleId"] = new SelectList(_context.Article, "Id", "Summary", picture.ArticleId);
            return View(picture);
        }

        // GET: Pictures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picture = await _context.Picture
                .Include(p => p.Article)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (picture == null)
            {
                return NotFound();
            }

            return View(picture);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var picture = await _context.Picture.FindAsync(id);
            _context.Picture.Remove(picture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PictureExists(int id)
        {
            return _context.Picture.Any(e => e.Id == id);
        }
    }
}
