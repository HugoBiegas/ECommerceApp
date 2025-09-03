using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models
{
    public class Cart
    {
        public int UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
        public int TotalItems => Items.Sum(item => item.Quantity);

        public void AddItem(Book book, int quantity = 1)
        {
            var existingItem = Items.FirstOrDefault(i => i.BookId == book.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    BookId = book.Id,
                    BookTitle = book.Title,
                    UnitPrice = book.Price,
                    Quantity = quantity
                });
            }
        }

        public void RemoveItem(int bookId)
        {
            Items.RemoveAll(i => i.BookId == bookId);
        }

        public void UpdateQuantity(int bookId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    RemoveItem(bookId);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }

}
