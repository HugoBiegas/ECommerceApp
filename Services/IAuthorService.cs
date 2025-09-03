using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<int> AddAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(Author author);
        Task<bool> DeleteAuthorAsync(int id);
        Task<bool> AuthorExistsAsync(int id);
        Task<bool> CanDeleteAuthorAsync(int id);
        Task<List<Author>> SearchAuthorsAsync(string searchTerm);
        Task<int> GetBooksCountByAuthorAsync(int authorId);
    }

}
