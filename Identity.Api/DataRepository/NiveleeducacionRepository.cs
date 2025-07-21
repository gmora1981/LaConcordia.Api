using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class NiveleeducacionRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public NiveleeducacionRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Niveleducacion> GetNiveleducacionInfoAll()
        {
            return _context.Niveleducacions.Where(x => x.Estado == "a").ToList();
        }

        public NiveleducacionDTO? GetNiveleducacionById(int idNiveleducacion)
        {
            var s = _context.Niveleducacions
                .FirstOrDefault(f => f.Ideducacion == idNiveleducacion);
            if (s == null) return null;
            return new NiveleducacionDTO
            {
                Ideducacion = s.Ideducacion,
                Descripcion = s.Descripcion,
                Estado = s.Estado
            };
        }

        public void InsertNiveleducacion(Niveleducacion nueva)
        {
            _context.Niveleducacions.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateNiveleducacion(Niveleducacion actualizada)
        {
            _context.Niveleducacions.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteNiveleducacionById(int idNiveleducacion)
        {
            var item = _context.Niveleducacions.FirstOrDefault(x => x.Ideducacion == idNiveleducacion);
            if (item != null)
            {
                _context.Niveleducacions.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
