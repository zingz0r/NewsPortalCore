using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Data.Entity;
using NewsPortal.ViewModels;

namespace NewsPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsPortalContext _context;

        public HomeController(NewsPortalContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var newsPortalContext = _context.Article.OrderByDescending(x => x.Date).Take(10).Include(a => a.Author);
            return View(await newsPortalContext.ToListAsync());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
