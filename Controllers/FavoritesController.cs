using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreMvc.Data;
using BookStoreMvc.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace BookStoreMvc.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Favoriler sayfası
        public IActionResult Index()
        {
            var favoriteIds = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            var favoriteBooks = _context.Books
                .Include(b => b.Category)
                .Where(b => favoriteIds.Contains(b.Id))
                .ToList();

            return View(favoriteBooks);
        }

        // Favorilere ekle (normal, redirect bazlı)
        public IActionResult AddToFavorites(int id)
        {
            var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            if (!favorites.Contains(id))
                favorites.Add(id);

            HttpContext.Session.SetObjectAsJson("Favorites", favorites);

            return RedirectToAction("Index", "Book"); // Kitaplar sayfasına döner
        }

        // Favorilerden çıkar (normal, redirect bazlı)
        public IActionResult RemoveFromFavorites(int id)
        {
            var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            if (favorites.Contains(id))
                favorites.Remove(id);

            HttpContext.Session.SetObjectAsJson("Favorites", favorites);

            return RedirectToAction("Index");
        }

        // AJAX ile favorilere ekleme
        [HttpPost]
        public JsonResult AddToFavoritesAjax(int id)
        {
            var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            if (!favorites.Contains(id))
                favorites.Add(id);

            HttpContext.Session.SetObjectAsJson("Favorites", favorites);

            return Json(new { success = true });
        }

        // AJAX ile favorilerden çıkarma
        [HttpPost]
        public JsonResult RemoveFromFavoritesAjax(int id)
        {
            var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            if (favorites.Contains(id))
                favorites.Remove(id);

            HttpContext.Session.SetObjectAsJson("Favorites", favorites);

            return Json(new { success = true });
        }
    }
}