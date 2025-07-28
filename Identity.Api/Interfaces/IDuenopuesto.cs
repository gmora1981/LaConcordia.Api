using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IDuenopuesto
    {
        IEnumerable<DuenopuestoDTO> GetDuenopuestoInfoAll();
        DuenopuestoDTO GetDuenopuestoById(string cedula);
        void InsertDuenopuesto(DuenopuestoDTO New);
        void UpdateDuenopuesto(DuenopuestoDTO UpdItem);
        void DeletePDuenopuestoById(string cedula);

        //paginado
        Task<PagedResult<Duenopuesto>> GetDuenopuestosPaginados(
            int pagina,
            int pageSize,
            string? cedula = null,
            string? nombre = null,
            string? apellidos = null,
            string? estado = null);

        //exportar
        List<Duenopuesto> ObtenerDuenoPuestoFiltradas(string? filtro);

    }

}
