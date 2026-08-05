using Identity.Api.DTO;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class GenerarMultaRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public GenerarMultaRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public List<GenerarMultaDTO> GetMultasPorSocio(string cidentidad)
        {
            using var context = new DbAa5796GmoraContext();

            return context.Generarmulta
                .Where(m => m.Cidentidad == cidentidad)
                .OrderBy(m => m.Idmulta)
                .Select(m => new GenerarMultaDTO
                {
                    Idmulta = m.Idmulta,
                    Cidentidad = m.Cidentidad,
                    Fecha = m.Fecha,
                    Observacion = m.Observacion,
                    Valor = m.Valor,
                    Abono = m.Abono,
                    Tipo = m.Tipo
                })
                .ToList();
        }

        public GenerarMultaDTO InsertGenerarMulta(GenerarMultaDTO nueva)
        {
            var ultimoId = _context.Generarmulta
                .Where(m => m.Cidentidad == nueva.Cidentidad)
                .Select(m => m.Idmulta)
                .ToList()
                .Select(id => int.TryParse(id, out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            var siguienteId = (ultimoId + 1).ToString();

            var entidad = new Generarmultum
            {
                Idmulta = siguienteId,
                Cidentidad = nueva.Cidentidad,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                Observacion = nueva.Observacion,
                Valor = nueva.Valor,
                Abono = 0m,
                Tipo = nueva.Tipo
            };

            _context.Generarmulta.Add(entidad);
            _context.SaveChanges();

            nueva.Idmulta = entidad.Idmulta;
            nueva.Fecha = entidad.Fecha;
            nueva.Abono = entidad.Abono;
            return nueva;
        }
    }
}
