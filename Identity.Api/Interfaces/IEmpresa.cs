using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IEmpresa
    {
        IEnumerable<Empresa> GetEmpresaInfoAll();
        EmpresaDTO GetEmpresaByRuc(string ruc);
        void InsertEmpresa(Empresa New);
        void UpdateEmpresa(Empresa UpdItem);
        void DeleteEmpresaByRuc(string ruc);

        //paginado
        Task<PagedResult<Empresa>> GetEmpresasPaginados(
        int pagina,
        int pageSize,
        string? ruc = null,
        string? razonsocial = null,
        string? telefono = null,
        string? estado = null);
    }
    
}
