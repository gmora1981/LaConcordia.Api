using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IUnidad
    {
        IEnumerable<Unidad> GetUnidadInfoAll();
        Unidad? GetUnidadById(string idUnidad);
        void InsertUnidad(Unidad nueva);
        void UpdateUnidad(Unidad actualizada);
        void DeleteUnidadById(string idUnidad);

    }
}
