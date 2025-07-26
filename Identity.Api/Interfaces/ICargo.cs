using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface ICargo
    {
        IEnumerable<Cargo> CargoInfoAll { get; }

        CargoDTO GetCargoById(int idCargo);
        void InsertCargo(Cargo New);
        void UpdateCargo(Cargo UpdItem);

        void DeleteCargo(Cargo DelItem);
        void DeleteCargo2(int idregistrado);

        //Paginado
        Task<PagedResult<Cargo>> GetCargoPaginados(
        int pagina,
        int pageSize,
        string? cargo1 = null,
        string? estado = null);
    }
}
