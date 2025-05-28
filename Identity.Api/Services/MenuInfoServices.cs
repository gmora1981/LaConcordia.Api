using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class MenuInfoServices : IMenuInfo
    {
        public MenuInfoDataRepository data = new MenuInfoDataRepository();
        public IEnumerable<MenuInfo> ListMenuInfoAll
        {
            get { return data.MenuInfoAll(); }
        }
    }
}
