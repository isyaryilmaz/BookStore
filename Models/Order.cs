using System;
using System.Collections.Generic;

namespace BookStoreMvc.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal TotalPrice { get; set; }
    }
}