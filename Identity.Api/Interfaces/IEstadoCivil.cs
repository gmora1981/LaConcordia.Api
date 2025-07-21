using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IEstadoCivil
    {
        IEnumerable<Estadocivil> GetEstadocivilInfoAll();
        EstadocivilDTO GetEstadocivilById(int idestadocivil);
        void InsertEstadocivil(Estadocivil New);
        void UpdateEstadocivil(Estadocivil UpdItem);
        void DeleteEstadocivilById(int idestadocivil);
    }
}
