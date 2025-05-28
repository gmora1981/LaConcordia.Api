using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface ICargo
    {
        IEnumerable<Cargo> CargoInfoAll { get; }
        void InsertCargo(Cargo New);
        void UpdateCargo(Cargo UpdItem);

        void DeleteCargo(Cargo DelItem);
        void DeleteCargo2(int idregistrado);

    }
}
