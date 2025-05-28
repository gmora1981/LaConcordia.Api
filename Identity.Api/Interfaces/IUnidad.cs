using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IUnidad
    {
        IEnumerable<Unidad> UnidadInfoAll { get; }
        IEnumerable<Unidad> UnidadXUnidad(string Item);
        void InsertUnidad(Unidad New);
    }
}
