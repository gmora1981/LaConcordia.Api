using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class TipolicenciumRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public TipolicenciumRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Tipolicencium> GetTipolicenciaInfoAll()
        {
            return _context.Tipolicencia.Where(x => x.Estado == "a").ToList();
        }

        public TipolicenciumDTO? GetTipolicenciaById(int idTipoLicencia)
        {
            //return _context.Parentescos.FirstOrDefault(x => x.Idparentesco == id && x.Estado == "a");
            using var context = new DbAa5796GmoraContext();

            var s = context.Tipolicencia
                //.Include(f => f.IdProveedorNavigation)
                //.Include(f => f.IdBodegaNavigation)
                .FirstOrDefault(f => f.Idtipo == idTipoLicencia);

            if (s == null) return null;

            return new TipolicenciumDTO
            {
                Idtipo = s.Idtipo,
                Tipolicencia = s.Tipolicencia,
                Profesional = s.Profesional,
                Estado = s.Estado
            };
        }

        public void InsertTipolicencia(Tipolicencium nueva)
        {
            _context.Tipolicencia.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateTipolicencia(Tipolicencium actualizada)
        {
            _context.Tipolicencia.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteTipolicenciaById(int idTipoLicencia)
        {
            var item = _context.Tipolicencia.FirstOrDefault(x => x.Idtipo == idTipoLicencia);
            if (item != null)
            {
                _context.Tipolicencia.Remove(item);
                _context.SaveChanges();
            }

        }
    }
}   
