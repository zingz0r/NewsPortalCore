using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Models;
using NewsPortal.Models.Entiy;

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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
