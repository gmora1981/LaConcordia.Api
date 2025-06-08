using Modelo.laconcordia.Modelo.Database;
using System.Linq.Expressions;

namespace Identity.Api.Interfaces
{
    public interface  INavigation
    {
        Task<IEnumerable<NavigationItem>> GetAllAsync();
        Task<IEnumerable<NavigationItem>> GetActiveTreeAsync();
        Task<NavigationItem?> GetByIdAsync(int id);
        Task<NavigationItem> CreateAsync(NavigationItem dto);
        Task UpdateAsync(NavigationItem dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<NavigationItem>> GetByRoleAsync(string role);
        Task<int> GetNextOrderAsync(int? parentId);
        Task ReorderItemsAsync(int? parentId);
        Task MoveItemAsync(int itemId, string direction);
    }
}
