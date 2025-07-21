using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class EstadoCivilRepository
    {
        private readonly DbAa5796GmoraContext _context;
        public EstadoCivilRepository()
        {
            _context = new DbAa5796GmoraContext();
        }
        public IEnumerable<Estadocivil> GetEstadocivilInfoAll()
        {
            return _context.Estadocivils.Where(x => x.Estado == "a").ToList();
        }
        public EstadocivilDTO GetEstadocivilById(int idEstadoCivil)
        {
            return _context.Estadocivils
                .Where(x => x.Idestadocivil == idEstadoCivil && x.Estado == "a")
                .Select(s => new EstadocivilDTO
                {
                    Idestadocivil = s.Idestadocivil,
                    Descripcion = s.Descripcion,
                    Estado = s.Estado
                })
                .FirstOrDefault();
        }
        public void InsertEstadocivil(Estadocivil nueva)
        {
            _context.Estadocivils.Add(nueva);
            _context.SaveChanges();
        }
        public void UpdateEstadocivil(Estadocivil actualizada)
        {
            _context.Estadocivils.Update(actualizada);
            _context.SaveChanges();
        }
        public void DeleteEstadocivilById(int idEstadoCivil)
        {
            var item = _context.Estadocivils.FirstOrDefault(x => x.Idestadocivil == idEstadoCivil);
            if (item != null)
            {
                _context.Estadocivils.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
