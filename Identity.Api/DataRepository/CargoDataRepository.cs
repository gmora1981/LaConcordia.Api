using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class CargoDataRepository
    {
        public List<Cargo> CargoAll()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return context.Cargos.ToList();
            }
        }

        public void InsertCargo(Cargo NewItem)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                context.Cargos.Add(NewItem);
                context.SaveChanges();
            }
        }

        public CargoDTO GetCargoById(int idCargo)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var cargo = context.Cargos
                    .Where(c => c.Idcargo == idCargo)
                    .Select(c => new CargoDTO
                    {
                        Idcargo = c.Idcargo,
                        Cargo1 = c.Cargo1,
                        Estado = c.Estado
                    })
                    .FirstOrDefault();
                return cargo;
            }
        }

        public void UpdateCargo(Cargo UpdItem)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var registrado = context.Cargos.Where(a => a.Idcargo == UpdItem.Idcargo).FirstOrDefault();

                if (registrado != null)
                {

                    registrado.Idcargo = UpdItem.Idcargo;
                    registrado.Cargo1 = UpdItem.Cargo1;
                    registrado.Estado = UpdItem.Estado;
                   
                    context.SaveChanges();

                }

            }
        }

        public void DeleteCargo(Cargo NewItem)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                context.Cargos.Remove(NewItem);
                context.SaveChanges();
            }
        }

        public void DeleteCargo2(int Idregistrado)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var registrado = context.Cargos.Where(a => a.Idcargo == Idregistrado).FirstOrDefault();

                if (registrado != null)
                {
                    context.Cargos.Remove(registrado);

                    context.SaveChanges();

                }

            }
        }


    }
}
