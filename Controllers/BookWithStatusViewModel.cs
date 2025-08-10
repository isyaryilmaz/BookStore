namespace BookStoreMvc.Models
{
    public class BookWithStatusViewModel
    {
        public Book Book { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsInCart { get; set; }
    }
}