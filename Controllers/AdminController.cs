using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreMvc.Data;
using BookStoreMvc.Models;
using System.Linq;

namespace BookStoreMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Tüm siparişleri listele
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .Include(o => o.User)
                .ToList();

            return View(orders);
        }
    }
}