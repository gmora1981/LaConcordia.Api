using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public  interface IParentesco
    {
        IEnumerable<Parentesco> GetParentescoInfoAll();
        ParentescoDTO GetParentescoById(int idParentesco);
        void InsertParentesco(Parentesco New);
        void UpdateParentesco(Parentesco UpdItem);
        void DeleteParentescoById(int idParentesco);

        //paginado
        Task<PagedResult<Parentesco>> GetParentescoPaginados(
        int pagina,
        int pageSize,
        string? parentesco1 = null,
        string? estado = null);
    }
}
