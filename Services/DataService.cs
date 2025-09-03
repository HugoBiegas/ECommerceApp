
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public class DataService : IDataService
    {
        private static bool _isInitialized = false;

        public async Task InitializeDataAsync()
        {
            if (_isInitialized) return;

            // Initialiser les données de test
            UserService.InitializeTestUsers();
            AuthorService.InitializeTestAuthors();
            BookService.InitializeTestBooks();

            _isInitialized = true;
            await Task.CompletedTask;
        }

        public async Task<bool> IsDataInitializedAsync()
        {
            return await Task.FromResult(_isInitialized);
        }

        public async Task ResetDataAsync()
        {
            // Cette méthode pourrait être utilisée pour réinitialiser toutes les données
            _isInitialized = false;

            // Ici on pourrait clear toutes les listes statiques des services
            // Mais pour la démo, on garde les données

            await Task.CompletedTask;
        }
    }

}
