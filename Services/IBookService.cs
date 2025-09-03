
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> SearchBooksAsync(BookListViewModel searchModel);
        Task<int> AddBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> UpdateStockAsync(int bookId, int newStock);
        Task<bool> IsBookAvailableAsync(int bookId, int quantity);
        Task<List<Book>> GetBooksByCategoryAsync(BookCategory category);
        Task<List<Book>> GetBooksByAuthorAsync(int authorId);
        Task<List<Book>> GetAvailableBooksAsync();
        Task<int> GetTotalBooksCountAsync();
        Task<List<Book>> GetTopSellingBooksAsync(int count = 5);
        Task<List<Book>> GetRecentBooksAsync(int count = 5);
        Task<bool> BookExistsAsync(int id);
    }

}
