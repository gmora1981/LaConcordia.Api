using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class NavigationServices : INavigation
    {
       public NavigationDataRepository data =  new NavigationDataRepository();

      

        public async Task<IEnumerable<NavigationItem>> GetAllAsync()
        {
            var items = await data.GetAllAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<NavigationItem>> GetActiveTreeAsync()
        {
            var items = await data.GetTreeAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<NavigationItem> GetByIdAsync(int id)
        {
            var item = await data.GetByIdAsync(id);
            return item != null ? MapToDto(item) : null;
        }

        public async Task<NavigationItem> CreateAsync(NavigationItem dto)
        {
            var item = new NavigationItem
            {
                ParentId = dto.ParentId,
                Title = dto.Title,
                Url = dto.Url,
                Icon = dto.Icon,
                Order = dto.Order,
                IsActive = dto.IsActive,
                RequiredRole = dto.RequiredRole
            };

            var created = await data.CreateAsync(item);
            return MapToDto(created);
        }

        public async Task UpdateAsync(NavigationItem dto)
        {
            var item = await data.GetByIdAsync(dto.Id);
            if (item == null)
                throw new InvalidOperationException($"Navigation item with ID {dto.Id} not found.");

            item.ParentId = dto.ParentId;
            item.Title = dto.Title;
            item.Url = dto.Url;
            item.Icon = dto.Icon;
            item.Order = dto.Order;
            item.IsActive = dto.IsActive;
            item.RequiredRole = dto.RequiredRole;

            await data.UpdateAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            await data.DeleteAsync(id);
        }

        public async Task<IEnumerable<NavigationItem>> GetByRoleAsync(string role)
        {
            var items = await data.FindAsync(n =>
                n.IsActive==true && (string.IsNullOrEmpty(n.RequiredRole) || n.RequiredRole == role)
            );

            return BuildTree(items.Select(MapToDto).ToList());
        }

        public async Task<int> GetNextOrderAsync(int? parentId)
        {
            // Necesitarías agregar este método a la interfaz o acceder directamente al repositorio
            if (data is NavigationDataRepository repo)
            {
                return await repo.GetNextOrderAsync(parentId);
            }
            return 10;
        }

        public async Task ReorderItemsAsync(int? parentId)
        {
            if (data is NavigationDataRepository repo)
            {
                await repo.ReorderItemsAsync(parentId);
            }
        }

        public async Task MoveItemAsync(int itemId, string direction)
        {
            var item = await data.GetByIdAsync(itemId);
            if (item == null) return;

            var siblings = await data.FindAsync(n =>
                n.ParentId == item.ParentId && n.Id != itemId);

            var siblingsList = siblings.OrderBy(s => s.Order).ToList();
            NavigationItem swapWith = null;

            bool moveUp = direction.ToLower() == "up";

            if (moveUp)
            {
                swapWith = siblingsList
                    .Where(s => s.Order < item.Order)
                    .OrderByDescending(s => s.Order)
                    .FirstOrDefault();
            }
            else
            {
                swapWith = siblingsList
                    .Where(s => s.Order > item.Order)
                    .OrderBy(s => s.Order)
                    .FirstOrDefault();
            }

            if (swapWith != null)
            {
                var tempOrder = item.Order;
                item.Order = swapWith.Order;
                swapWith.Order = tempOrder;

                await data.UpdateAsync(item);
                await data.UpdateAsync(swapWith);
            }
        }

        private NavigationItem MapToDto(NavigationItem item)
        {
            return new NavigationItem
            {
                Id = item.Id,
                ParentId = item.ParentId,
                Title = item.Title,
                Url = item.Url,
                Icon = item.Icon,
                Order = item.Order,
                IsActive = item.IsActive,
                RequiredRole = item.RequiredRole,
                InverseParent = item.InverseParent?.Select(MapToDto).OrderBy(c => c.Order).ToList() ?? new List<NavigationItem>()
            };
        }

        private List<NavigationItem> BuildTree(List<NavigationItem> allItems)
        {
            var lookup = allItems.ToLookup(i => i.ParentId);

            foreach (var item in allItems)
            {
                item.InverseParent = lookup[item.Id].OrderBy(c => c.Order).ToList();
            }

            return lookup[null].OrderBy(i => i.Order).ToList();
        }
    }
}
