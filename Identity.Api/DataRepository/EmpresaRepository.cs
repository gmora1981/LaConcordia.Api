using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class EmpresaRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public EmpresaRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Empresa> GetEmpresaInfoAll()
        {
            return _context.Empresas.Where(x => x.Estado == "a").ToList();
        }

        public EmpresaDTO GetEmpresaByRuc(string ruc)
        {
            //return _context.Parentescos.FirstOrDefault(x => x.Idparentesco == id && x.Estado == "a");
            using var context = new DbAa5796GmoraContext();

            var s = context.Empresas
                //.Include(f => f.IdProveedorNavigation)
                //.Include(f => f.IdBodegaNavigation)
                .FirstOrDefault(f => f.Ruc == ruc);

            if (s == null) return null;

            return new EmpresaDTO
            {
                Ruc = s.Ruc,
                Razonsocial = s.Razonsocial,
                Direccion = s.Direccion,
                Telefono = s.Telefono,
                Estado = s.Estado
            };
            
        }

        public void InsertEmpresa(Empresa nueva)
        {
            _context.Empresas.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateEmpresa(Empresa actualizada)
        {
            _context.Empresas.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteEmpresaByRuc(string ruc)
        {
            var item = _context.Empresas.FirstOrDefault(x => x.Ruc == ruc);
            if (item != null)
            {
                _context.Empresas.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}

