using Identity.Api.DTO;
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
    }
    
}
