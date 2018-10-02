using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Models.Entiy;

namespace NewsPortal.Controllers
{
    public class ManagerController : Controller
    {
        private readonly NewsPortalContext _context;

        public ManagerController(NewsPortalContext context)
        {
            _context = context;
        }

        public FileResult GetImage(Int32? Id, Boolean IsLarge = false)
        {
            if (Id == null) // nem adtak meg azonosítót
                return File("~/images/NoImage.png", "image/png");

            // lekérjük a megadott azonosítóval rendelkező képet
            Byte[] imageContent = _context.Picture
                .Where(image => image.Id == Id)
                .Select(image => IsLarge ? image.LargeImageData : image.SmallImageData)
                .FirstOrDefault();

            if (imageContent == null) // amennyiben nem sikerült betölteni, egy alapértelmezett képet adunk vissza
                return File("~/images/NoImage.png", "image/png");

            return File(imageContent, "image/png");
        }
    }
}