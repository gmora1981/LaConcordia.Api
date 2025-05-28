using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class MenuInfoDataRepository
    {
        public List<MenuInfo> MenuInfoAll()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return context.MenuInfos.ToList();
            }
        }
    }
}
