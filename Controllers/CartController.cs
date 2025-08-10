using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreMvc.Data;
using BookStoreMvc.Extensions;
using BookStoreMvc.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BookStoreMvc.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cartIds = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();

            var cartBooks = _context.Books
                .Include(b => b.Category)
                .Where(b => cartIds.Contains(b.Id))
                .ToList();

            ViewBag.TotalPrice = cartBooks.Sum(b => b.Price);

            return View(cartBooks);
        }

        public IActionResult AddToCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();

            if (!cart.Contains(id))
                cart.Add(id);

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index", "Book");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();

            cart.Remove(id);

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToFavorites(int id)
        {
            var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            if (!favorites.Contains(id))
                favorites.Add(id);

            HttpContext.Session.SetObjectAsJson("Favorites", favorites);

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveFromFavorites(int id)
        {
            var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

            if (favorites.Contains(id))
                favorites.Remove(id);

            HttpContext.Session.SetObjectAsJson("Favorites", favorites);

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            var cartIds = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();

            if (!cartIds.Any())
            {
                TempData["OrderMessage"] = "Sepetiniz boş, sipariş verilemedi.";
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["OrderMessage"] = "Sipariş vermek için giriş yapmalısınız.";
                return RedirectToAction("Index");
            }

            var booksInCart = _context.Books
                .Where(b => cartIds.Contains(b.Id))
                .ToList();

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = booksInCart.Sum(b => b.Price)
            };

            foreach (var book in booksInCart)
            {
                order.Items.Add(new OrderItem
                {
                    BookId = book.Id,
                    Price = book.Price,
                    Quantity = 1,
                    Order = order
                });
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            HttpContext.Session.Remove("Cart");

            TempData["OrderMessage"] = "Siparişiniz başarıyla alındı!";

            return RedirectToAction("Index");
        }
    }
}