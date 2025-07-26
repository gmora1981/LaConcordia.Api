using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class CargoServices : ICargo 
    {
        public CargoDataRepository data = new CargoDataRepository();
        public IEnumerable<Cargo> CargoInfoAll
        {
            get { return data.CargoAll(); }
        }

        public CargoDTO GetCargoById(int idCargo)
        {
            return data.GetCargoById(idCargo);
        }

        public void InsertCargo(Cargo New)
        {
           data.InsertCargo(New);
        }

        public void UpdateCargo(Cargo UpdItem)
        {

            data.UpdateCargo(UpdItem);
        }
        
        public void DeleteCargo(Cargo DelItem)
        {
            data.DeleteCargo(DelItem);
        }

        public void DeleteCargo2(int idregistrado)
        {
            data.DeleteCargo2(idregistrado);
        }


        //paginado
        public async Task<PagedResult<Cargo>> GetCargoPaginados(int pagina,
            int pageSize,  string? cargo1 = null, string? estado = null)
        {
            return await data.GetCargoPaginados(pagina, pageSize, cargo1, estado);
        }
    }
}
