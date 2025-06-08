using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;
using System.Linq.Expressions;

namespace Identity.Api.DataRepository
{
    public class NavigationDataRepository
    {
        public async Task<IEnumerable<NavigationItem>> GetAllAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems
                    .Include(n => n.InverseParent)
                    .OrderBy(n => n.Order)
                    .ToListAsync() ?? new List<NavigationItem>();
            }
        }

        public async Task<IEnumerable<NavigationItem>> GetActiveItemsAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems
                    .Include(n => n.InverseParent)
                    .Where(n => n.IsActive==true)
                    .OrderBy(n => n.Order)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<NavigationItem>> GetTreeAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var allItems = await context.NavigationItems
                    .Where(n => n.IsActive==true)
                    .OrderBy(n => n.Order)
                    .ToListAsync();

                // Construir árbol
                var itemDict = allItems.ToDictionary(i => i.Id);
                var rootItems = new List<NavigationItem>();

                foreach (var item in allItems)
                {
                    if (item.ParentId == null)
                    {
                        rootItems.Add(item);
                    }
                    else if (itemDict.ContainsKey(item.ParentId.Value))
                    {
                        var parent = itemDict[item.ParentId.Value];
                        if (parent.InverseParent == null)
                            parent.InverseParent = new List<NavigationItem>();
                        parent.InverseParent.Add(item);
                    }
                }

                return rootItems;
            }
        }

        public async Task<NavigationItem> GetByIdAsync(int id)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems
                    .Include(n => n.Parent)
                    .Include(n => n.InverseParent)
                    .FirstOrDefaultAsync(n => n.Id == id);
            }
        }

        public async Task<NavigationItem?> GetWithChildrenAsync(int id)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                NavigationItem? navigationItemWithChildren = await context.NavigationItems
                    .Include(n => n.InverseParent)
                        .ThenInclude(c => c.InverseParent)
                    .FirstOrDefaultAsync(n => n.Id == id);
                return navigationItemWithChildren; // Fix: Explicitly mark the return type as nullable (NavigationItem?).
            }
        }

        public async Task<IEnumerable<NavigationItem>> FindAsync(Expression<Func<NavigationItem, bool>> predicate)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems
                    .Where(predicate)
                    .ToListAsync();
            }
        }

        public async Task<NavigationItem> CreateAsync(NavigationItem item)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                item.CreatedDate = DateTime.UtcNow;
                item.ModifiedDate = DateTime.UtcNow;

                // Si no tiene orden, obtener el siguiente
                if (item.Order == 0)
                {
                    var maxOrder = await context.NavigationItems
                        .Where(n => n.ParentId == item.ParentId)
                        .MaxAsync(n => (int?)n.Order) ?? 0;
                    item.Order = maxOrder + 10;
                }

                context.NavigationItems.Add(item);
                await context.SaveChangesAsync();
                return item;
            }
        }

        public async Task UpdateAsync(NavigationItem item)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var registrado = await context.NavigationItems
                    .Where(a => a.Id == item.Id)
                    .FirstOrDefaultAsync();

                if (registrado != null)
                {
                    registrado.ParentId = item.ParentId;
                    registrado.Title = item.Title;
                    registrado.Url = item.Url;
                    registrado.Icon = item.Icon;
                    registrado.Order = item.Order;
                    registrado.IsActive = item.IsActive;
                    registrado.RequiredRole = item.RequiredRole;
                    registrado.ModifiedDate = DateTime.UtcNow;

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var registrado = await context.NavigationItems
                    .Where(a => a.Id == id)
                    .FirstOrDefaultAsync();

                if (registrado != null)
                {
                    // Verificar si tiene hijos
                    var hasChildren = await context.NavigationItems
                        .AnyAsync(n => n.ParentId == id);

                    if (!hasChildren)
                    {
                        context.NavigationItems.Remove(registrado);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems.AnyAsync(e => e.Id == id);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.SaveChangesAsync();
            }
        }

        // Métodos adicionales útiles (no en la interfaz)
        public async Task<int> GetNextOrderAsync(int? parentId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var maxOrder = await context.NavigationItems
                    .Where(n => n.ParentId == parentId)
                    .MaxAsync(n => (int?)n.Order) ?? 0;

                return maxOrder + 10;
            }
        }

        public async Task ReorderItemsAsync(int? parentId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var items = await context.NavigationItems
                    .Where(n => n.ParentId == parentId)
                    .OrderBy(n => n.Order)
                    .ToListAsync();

                int order = 10;
                foreach (var item in items)
                {
                    item.Order = order;
                    item.ModifiedDate = DateTime.UtcNow;
                    order += 10;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
