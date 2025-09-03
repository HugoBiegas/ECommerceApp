
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface IDataService
    {
        Task InitializeDataAsync();
        Task<bool> IsDataInitializedAsync();
        Task ResetDataAsync();
    }

}
