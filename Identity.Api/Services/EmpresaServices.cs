using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class EmpresaServices : IEmpresa
    {
        private EmpresaRepository _empresa = new EmpresaRepository();


        public IEnumerable<Empresa> GetEmpresaInfoAll()
        {
            return _empresa.GetEmpresaInfoAll();
        }

        public EmpresaDTO GetEmpresaByRuc(string ruc)
        {
            return _empresa.GetEmpresaByRuc(ruc);
        }

        public void InsertEmpresa(Empresa New)
        {
            _empresa.InsertEmpresa(New);
        }

        public void UpdateEmpresa(Empresa UpdItem)
        {
            _empresa.UpdateEmpresa(UpdItem);
        }

        public void DeleteEmpresaByRuc(string ruc)
        {
            _empresa.DeleteEmpresaByRuc(ruc);
        }

        //paginado
        // Servicio EmpresaService
        public async Task<PagedResult<Empresa>> GetEmpresasPaginados(int pagina, int pageSize, string? ruc = null, string? razonsocial = null, string? telefono = null, string? estado = null)
        {
            return await _empresa.GetEmpresasPaginados(pagina, pageSize, ruc, razonsocial, telefono, estado);
        }
    }
}
