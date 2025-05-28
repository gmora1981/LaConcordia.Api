using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
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



    }
}
