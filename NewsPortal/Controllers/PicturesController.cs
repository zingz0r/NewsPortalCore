using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Data.Entity;
using NewsPortal.PageModels;

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
        public async Task<IActionResult> Create(PictureUploadPageModel pictureUploadViewModel)
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
    }
}
