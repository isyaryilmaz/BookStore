using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreMvc.Data;
using BookStoreMvc.Models;
using System.Linq;

namespace BookStoreMvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories
                .Include(c => c.Books)
                .ToList();

            return View(categories);
        }

        public IActionResult Details(int id)
        {
            var category = _context.Categories
                .Include(c => c.Books)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}