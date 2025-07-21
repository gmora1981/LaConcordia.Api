using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface INacionalidad
    {
        IEnumerable<Nacionalidad> GetNacionalidadInfoAll();
        NacionalidadDTO? GetNacionalidadById(int idNacionalidad);
        void InsertNacionalidad(Nacionalidad nueva);
        void UpdateNacionalidad(Nacionalidad actualizada);
        void DeleteNacionalidadById(int idNacionalidad);

    }
}
