using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class UnidadDataRepository
    {

        private readonly DbAa5796GmoraContext _context;

        public UnidadDataRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Unidad> UnidadAll()
        {
            return _context.Unidads.Where(x => x.Estado == "a").ToList();
        }

        public IEnumerable<Unidad> UnidadXUnidad(string idUnidad)
        {
            return _context.Unidads.Where(x => x.Unidad1 == idUnidad && x.Estado == "a").ToList();
        }

        public void InsertUnidad(Unidad nueva)
        {
            _context.Unidads.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateUnidad(Unidad actualizada)
        {
            _context.Unidads.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteUnidadById(string idUnidad)
        {
            var item = _context.Unidads.FirstOrDefault(x => x.Unidad1 == idUnidad);
            if (item != null)
            {
                _context.Unidads.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
