using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreMvc.Data;
using BookStoreMvc.Models;
using BookStoreMvc.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace BookStoreMvc.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var books = _context.Books.Include(b => b.Category).ToList();

            var favoriteIds = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();
            var cartIds = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();

            var model = books.Select(b => new BookWithStatusViewModel
            {
                Book = b,
                IsFavorite = favoriteIds.Contains(b.Id),
                IsInCart = cartIds.Contains(b.Id)
            }).ToList();

            return View(model);
        }
    }
}
