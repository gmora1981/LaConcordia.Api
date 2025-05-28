using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IMenuInfo
    {
        IEnumerable<MenuInfo> ListMenuInfoAll { get; }
    }
}
